using Microsoft.AspNetCore.Mvc;
using OrderService.Data.DTO;
using OrderService.Data.Services;
using static Shared.Constants;

namespace OrderService.Controllers
{
    /// <summary>
    ///     Order controller
    /// </summary>
    [ApiController]
    [Route(ORDER_CONTROLLER_ROUTE)]
    public class OrderController(IOrderService s): Controller
    {
        private readonly IOrderService service = s;

        [HttpPost]
        [Route(POST_ORDER_ROUTE)]
        public async Task<ActionResult> AddOrder(string userId, [FromBody] OrderDTO order)
        {
            var result = await service.AddOrder(userId, order);
            return new JsonResult(result)
            {
                StatusCode = result.Code
            };
        }
    }
}
