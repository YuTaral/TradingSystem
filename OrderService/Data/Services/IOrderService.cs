using SharedData.Models;
using OrderService.Data.Entities;
using SharedData.Models.DTO;

namespace OrderService.Data.Services
{
    /// <summary>
    ///     Interface to define OrderService methods
    /// </summary>
    public interface IOrderService
    {

        /// <summary>
        ///     Save the order to the database
        /// </summary>
        /// <param name="userIdStr">
        ///     The user id who created the order
        /// </param>
        /// <param name="orderDTO">
        ///     OrderDTO object send in the request body
        /// </param>
        public Task<ServiceActionResult<Order>> AddOrder(string userIdStr, OrderDTO? orderDTO);
    }
}
