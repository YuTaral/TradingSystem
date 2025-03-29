using PortfolioService.Data.DTO;
using Shared.Models;

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
    }
}
