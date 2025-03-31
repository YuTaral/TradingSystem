using SharedData.Models;
using System.Net;
using static SharedData.Constants;
using OrderService.Data.Entities;
using Shared.Utils;
using SharedData.Models.DTO;
using System.Text.Json;

namespace OrderService.Data.Services
{
    /// <summary>
    ///     OrderService class implementing IOrderService
    /// </summary>
    public class OrderService(OrderDBContext DB, PriceConsumer ps, RabbitMQClient c) : IOrderService
    {
        private readonly OrderDBContext DBContext = DB;
        private readonly PriceConsumer priceConsumer = ps;
        private readonly RabbitMQClient client = c;

        public async Task<ServiceActionResult<Order>> AddOrder(string userIdStr, OrderDTO? orderDTO)
        {
            var validationResult = await ValidateOrder(userIdStr, orderDTO);

            if (validationResult.Code != (int) HttpStatusCode.OK) 
            {
                return new ServiceActionResult<Order>(validationResult.Code, validationResult.Message, []);
            }

            await DBContext.Orders.AddAsync(validationResult.Data[0]);
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
        private async Task<ServiceActionResult<Order>> ValidateOrder(string userIdStr, OrderDTO? orderDTO)
        {
            long userId = 0;

            // Validate the data
            if (string.IsNullOrEmpty(userIdStr) || orderDTO == null)
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, ADD_ORDER_INVALID_DATA, []);
            }
            else if (!long.TryParse(userIdStr, out userId))
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, ADD_ORDER_INVALID_DATA, []);
            }

            // Get the latest price of the stock, if invalid ticker is provided -1 is returned
            var stockPrice = priceConsumer.GetStockPrice(orderDTO.Ticker);
            if (stockPrice == -1)
            {
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, ADD_ORDER_FAILED_TO_GET_PRICE, []);
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
                return new ServiceActionResult<Order>(HttpStatusCode.BadRequest, errors, []);
            }

            orderDTO.userId = userId;

            // Create new RPC event and send the event
            var rPCEvent = new RPCEvent(RPCEventTypes.UPDATE_PORTFOLIO, orderDTO);

            var response = await client.CallAsync(rPCEvent.ToString());

            if (response.Code != (int) HttpStatusCode.OK) {
                return new ServiceActionResult<Order>(response.Code, response.Message, []);
            }

            return new ServiceActionResult<Order>(HttpStatusCode.OK, "", [order]);
        }
    }
}
