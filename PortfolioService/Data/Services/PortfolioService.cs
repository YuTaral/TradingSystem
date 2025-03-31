using Microsoft.EntityFrameworkCore;
using PortfolioService.Data.DTO;
using PortfolioService.Data.Entities;
using SharedData.Models;
using SharedData.Models.DTO;
using Shared.Utils;
using System.Net;
using static SharedData.Constants;

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
                return new ServiceActionResult<PortfolioDTO>(HttpStatusCode.BadRequest, USER_ID_ERROR, []);
            }

            // Get list of user's owned stocks
            var stocks = await DBContext.Portfolios.Where(p => p.UserId.Equals(userId)).ToListAsync();

            if (stocks.Count == 0)
            {
                return new ServiceActionResult<PortfolioDTO>(HttpStatusCode.NotFound, USER_PORTFOLIO_NOT_FOUND, []);
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
                [new PortfolioDTO { UserId = userId, Value = Math.Round(totalValue, 2) }]);
        }

        public async Task<ServiceActionResult<string>> UpdatePortfolio(OrderDTO order)
        {
            // Check whether the user has record for this stock
            var portfolio = await DBContext.Portfolios
                            .Where(p => p.UserId == order.userId && p.Ticker == order.Ticker)
                            .FirstOrDefaultAsync();

            if (order.Side == OrderSide.SELL.ToString())
            {
                return await SellStock(portfolio, order);
            } 
            else
            {
                return await BuyStock(portfolio, order);
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
        public async Task<ServiceActionResult<string>> BuyStock(Portfolio? portfolio, OrderDTO order)
        {
            if (portfolio == null)
            {
                // Create the record
                var record = new Portfolio
                {
                    UserId = order.userId,
                    Ticker = order.Ticker,
                    Quantity = order.Quantity,
                };

                var errors = Helper.ValidateModel(record);

                if (!string.IsNullOrEmpty(errors))
                {
                    return new ServiceActionResult<string>(HttpStatusCode.BadRequest, errors, []);
                }

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

            return new ServiceActionResult<string>(HttpStatusCode.OK, "", []);
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
        public async Task<ServiceActionResult<string>> SellStock(Portfolio? portfolio, OrderDTO order)
        {
            if (portfolio == null)
            {
                // User does not own the stock and is trying to sell it, throw error
                return new ServiceActionResult<string>(HttpStatusCode.BadRequest, PORTFOLIO_STOCK_NOT_FOUND, []);
            } 
            else if (order.Quantity > portfolio.Quantity)
            {
                // Not enough quantity
                return new ServiceActionResult<string>(HttpStatusCode.BadRequest, PORTFOLIO_INVALID_QUANTITY, []);
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

            return new ServiceActionResult<string>(HttpStatusCode.OK, "", []);
        }
    }
}
