using Microsoft.EntityFrameworkCore;
using PortfolioService.Data.DTO;
using Shared.Models;
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
    }
}
