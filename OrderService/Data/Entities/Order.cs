using System.ComponentModel.DataAnnotations;
using static SharedData.Constants;

namespace OrderService.Data.Entities
{
    /// <summary>
    ///     Order class representing a single row of Orders table
    /// </summary>
    public class Order
    {
        [Key]
        public long Id { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = USER_ID_ERROR)]
        public required long UserId { get; set; }

        [MinLength(1, ErrorMessage = STOCK_TICKER_IS_MANDATORY_ERROR)]
        [MaxLength(20, ErrorMessage = STOCK_TICKER_MAX_LEN_ERROR)]
        public required string Ticker { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = ORDER_QUANTITY_RANGE_ERROR)]
        public required int Quantity { get; set; }

        [EnumDataType(typeof(OrderSide), ErrorMessage = ORDER_SIDE_ERROR)]
        [MinLength(3, ErrorMessage = ORDER_SIDE_ERROR)]
        public required string Side { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = ORDER_STOCK_PRICE_RANGE_ERROR)]
        public required decimal Price { get; set; }
    }
}
