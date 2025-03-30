using System.ComponentModel.DataAnnotations;
using static Shared.Constants;

namespace PortfolioService.Data.Entities
{
    /// <summary>
    ///     Portfolio class representing a single row of Portfolios table
    /// </summary>
    public class Portfolio
    {

        [Key]
        public long Id { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = USER_ID_ERROR)]
        public required long UserId { get; set; }

        [MinLength(1, ErrorMessage = STOCK_TICKER_IS_MANDATORY_ERROR)]
        [MaxLength(20, ErrorMessage = STOCK_TICKER_MAX_LEN_ERROR)]
        public required string Ticker { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = PORTFOLIO_STOCK_QUANTITY_RANGE_ERROR)]
        public required int Quantity { get; set; }
    }
}
