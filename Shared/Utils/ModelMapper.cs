using SharedData.Models.DTO;

namespace Shared.Utils
{
    /// <summary>
    ///     Custom mapper class to map models to data transfer objects
    /// </summary>
    public static class ModelMapper
    {
        /// <summary>
        ///     Map the provided ticker and price to StockDTO
        /// </summary>
        /// <param name="ticker">
        ///     The ticker (e.g TSLA)
        /// </param>
        /// <param name="price">
        ///     Current price of the stock
        /// </param>
        /// <returns></returns>
        public static StockDTO MapToStockDTO(string ticker, decimal price)
        {
            return new StockDTO
            {
                Ticker = ticker,
                Price = price
            };
        }
    }
}
