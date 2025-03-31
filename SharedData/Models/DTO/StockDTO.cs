namespace SharedData.Models.DTO
{
    /// <summary>
    ///     StockDTO representing a single stock
    /// </summary>
    public class StockDTO
    {
        public required string Ticker { get; set; }
        public required decimal Price { get; set; }
    }
}
