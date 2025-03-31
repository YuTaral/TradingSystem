using Microsoft.AspNetCore.Mvc;
using PortfolioService.Data.Services;
using static SharedData.Constants;

namespace PortfolioService.Controllers
{
    /// <summary>
    ///     User protfolio controller
    /// </summary>
    [ApiController]
    [Route(PORTFOLIO_CONTROLLER_ROUTE)]
    public class PortfolioController(IPortfolioService s): Controller
    {
        private readonly IPortfolioService portfolioService = s;
        
        [HttpGet]
        [Route(GET_PORTFOLIO)]
        public async Task<ActionResult> GetPortfolio(string userId)
        {
            var result = await portfolioService.GetPortfolio(userId);

            return new JsonResult(result)
            {
                StatusCode = result.Code
            };
        }
    }
}
