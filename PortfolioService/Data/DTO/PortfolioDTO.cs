namespace PortfolioService.Data.DTO
{
    /// <summary>
    ///     PortfolioDTO used to transfer portfolio object
    /// </summary>
    public class PortfolioDTO
    {
        public required long UserId { get; set; }
        public required decimal Value { get; set; }
    }
}
