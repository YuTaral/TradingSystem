using Shared.Models;
using OrderService.Data.DTO;
using System.Net;
using static Shared.Constants;
using OrderService.Data.Entities;
using Shared.Utils;

namespace OrderService.Data.Services
{
    /// <summary>
    ///     OrderService class implementing IOrderService
    /// </summary>
    public class OrderService(AppDBContext DB, PriceConsumer ps) : IOrderService
    {
        private readonly AppDBContext DBContext = DB;
        private readonly PriceConsumer priceConsumer = ps;

        public async Task<ServiceActionResult<Order>> AddOrder(string userIdStr, OrderDTO? orderDTO)
        {
            var validationResult = ValidateOrder(userIdStr, orderDTO);

            if (validationResult.Code != (int) HttpStatusCode.OK) 
            {
                return new ServiceActionResult<Order>(validationResult.Code, validationResult.Message, null);
            }

            await DBContext.Orders.AddAsync(validationResult.Data!);
            await DBContext.SaveChangesAsync();

            return new ServiceActionResult<Order>(HttpStatusCode.Created, "", validationResult.Data);
        }

        /// <summary>
        ///     Validate the order data is valid
        /// </summary>
        /// <param name="userIdStr">
        ///     The user id who created the order
        /// </param>
        /// <param name="orderDTO">
        ///     OrderDTO object send in the request body
        /// </param>
        private ServiceActionResult<Order> ValidateOrder(string userIdStr, OrderDTO? orderDTO)
        {
            long userId = 0;

            // Validate the data
            if (string.IsNullOrEmpty(userIdStr) || orderDTO == null)
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, ADD_ORDER_INVALID_DATA, null);
            }
            else if (!long.TryParse(userIdStr, out userId))
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, ADD_ORDER_INVALID_DATA, null);
            }

            // Get the latest price of the stock, if invalid ticker is provided -1 is returned
            var stockPrice = priceConsumer.GetStockPrice(orderDTO.Ticker);
            if (stockPrice == -1)
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, ADD_ORDER_FAILED_TO_GET_PRICE, null);
            }

            var order = new Order
            {
                UserId = userId,
                Ticker = orderDTO.Ticker,
                Quantity = orderDTO.Quantity,
                Side = orderDTO.Side,
                Price = stockPrice
            };

            // Validate the model's annotations
            var errors = Helper.ValidateModel(order);

            if (!string.IsNullOrEmpty(errors))
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, errors, null);
            }

            return new ServiceActionResult<Order>(HttpStatusCode.OK, "", order);
        }
    }
}
