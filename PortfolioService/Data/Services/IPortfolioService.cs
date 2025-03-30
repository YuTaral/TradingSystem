using PortfolioService.Data.DTO;
using Shared.Models;
using Shared.Models.DTO;

namespace PortfolioService.Data.Services
{
    /// <summary>
    ///     Interface to define PortfolioService methods
    /// </summary>
    public interface IPortfolioService
    {
        /// <summary>
        ///     Return the portfolio of the user
        /// </summary>
        /// <param name="userIdStr">
        ///     The user id
        /// </param>
        public Task<ServiceActionResult<PortfolioDTO>> GetPortfolio(string userIdStr);


        /// <summary>
        ///     Update user's portfolio
        /// </summary>
        /// <param name="order">
        ///     The the order which is being executed
        /// </param>
        public Task<ServiceActionResult<string>> UpdatePortfolio(OrderDTO order);
    }
}
