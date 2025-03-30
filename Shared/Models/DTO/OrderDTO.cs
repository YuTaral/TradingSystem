namespace Shared.Models.DTO
{
    /// <summary>
    ///     OrderDTO class representing a single order
    /// </summary>
    public class OrderDTO
    {
        public required string Ticker { get; set; }
        public required int Quantity { get; set; }
        public required string Side { get; set; }
    }
}
