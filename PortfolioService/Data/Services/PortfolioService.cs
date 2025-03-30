using Microsoft.EntityFrameworkCore;
using PortfolioService.Data.DTO;
using PortfolioService.Data.Entities;
using Shared.Models;
using Shared.Models.DTO;
using System.Net;
using static Shared.Constants;

namespace PortfolioService.Data.Services
{
    /// <summary>
    ///     PortfolioService class implementing IPortfolioService
    /// </summary>
    public class PortfolioService(PortfolioDBContext DB, PriceConsumer ps) : IPortfolioService
    {
        private readonly PortfolioDBContext DBContext = DB;
        private readonly PriceConsumer priceConsumer = ps;

        public async Task<ServiceActionResult<PortfolioDTO>> GetPortfolio(string userIdStr)
        {
            // Validate
            if (!long.TryParse(userIdStr, out var userId))
            {
                return new ServiceActionResult<PortfolioDTO>(HttpStatusCode.BadRequest, USER_ID_ERROR, null);
            }

            // Get list of user's owned stocks
            var stocks = await DBContext.Portfolios.Where(p => p.UserId.Equals(userId)).ToListAsync();

            if (stocks.Count == 0)
            {
                return new ServiceActionResult<PortfolioDTO>(HttpStatusCode.NotFound, USER_PORTFOLIO_NOT_FOUND, null);
            }

            decimal totalValue = 0;
            decimal stockPrice = 0;

            // Calculate the total value of the user's portfolio
            foreach (var stock in stocks)
            {
                stockPrice = priceConsumer.GetStockPrice(stock.Ticker);

                if (stockPrice > -1)
                {
                    totalValue += stockPrice * stock.Quantity;
                }
            }

            return new ServiceActionResult<PortfolioDTO>(HttpStatusCode.OK, "", 
                new PortfolioDTO { UserId = userId, Value = Math.Round(totalValue, 2) });
        }

        public async Task<ServiceActionResult<bool>> UpdatePortfolio(long userId, OrderDTO order)
        {
            // Check whether the user has record for this stock
            var portfolio = await DBContext.Portfolios
                            .Where(p => p.UserId == userId && p.Ticker == order.Ticker)
                            .FirstOrDefaultAsync();

            if (order.Side == OrderSide.SELL.ToString())
            {
                return await SellStock(portfolio, order, userId);
            } 
            else
            {
                return await BuyStock(portfolio, order, userId);
            }
        }

        /// <summary>
        ///     Execute buy and update the user's portfolio for the given stock
        /// </summary>
        /// <param name="portfolio">
        ///     The portfolio record, may be null if the user does not yet own the stock
        /// </param>
        /// <param name="order">
        ///     The order containing the stock's ticker and quantity
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        /// <returns></returns>
        public async Task<ServiceActionResult<bool>> BuyStock(Portfolio? portfolio, OrderDTO order, long userId)
        {
            if (portfolio == null)
            {
                // Create the record
                var record = new Portfolio
                {
                    UserId = userId,
                    Ticker = order.Ticker,
                    Quantity = order.Quantity,
                };

                await DBContext.Portfolios.AddAsync(record);
            }
            else
            {
                // Update the record
                portfolio.Quantity += order.Quantity;
                DBContext.Entry(portfolio).State = EntityState.Modified;
            }

            // Save the add/update changes
            await DBContext.SaveChangesAsync();

            return new ServiceActionResult<bool>(HttpStatusCode.OK, "", true);
        }

        /// <summary>
        ///     Execute sell and update the user's portfolio for the given stock
        /// </summary>
        /// <param name="portfolio">
        ///     The portfolio record, may be null if the user does not yet own the stock
        /// </param>
        /// <param name="order">
        ///     The order containing the stock's ticker and quantity
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        /// <returns></returns>
        public async Task<ServiceActionResult<bool>> SellStock(Portfolio? portfolio, OrderDTO order, long userId)
        {
            if (portfolio == null)
            {
                // User does not own the stock and is trying to sell it, throw error
                return new ServiceActionResult<bool>(HttpStatusCode.BadRequest, PORTFOLIO_STOCK_NOT_FOUND, false);
            } 
            else if (order.Quantity > portfolio.Quantity)
            {
                // Not enough quantity
                return new ServiceActionResult<bool>(HttpStatusCode.BadRequest, PORTFOLIO_INVALID_QUANTITY, false);
            }
         
            if (order.Quantity == portfolio.Quantity)
            {
                // Updated quantity will be 0, remove the record
                DBContext.Remove(portfolio);
            } 
            else
            {
                // Quantity is enough, update the record
                portfolio.Quantity -= order.Quantity;
                DBContext.Entry(portfolio).State = EntityState.Modified;
            }
            
            await DBContext.SaveChangesAsync();

            return new ServiceActionResult<bool>(HttpStatusCode.OK, "", true);
        }
    }
}
