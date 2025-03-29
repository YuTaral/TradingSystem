using Shared.Models;
using Shared.Services;
using System.Net;
using static Shared.Constants;

namespace OrderService.Data.Services
{
    /// <summary>
    ///     OrderServicePriceConsumer as background service.
    ///     Use the shared PriceConsumerService to update the stock prices
    /// </summary>
    public class OrderServicePriceConsumer: BackgroundService
    {
        private const string CONSUMER_GROUP_ID = "group_1";
        private readonly SharedPriceConsumerService sharedPriceConsumerService;

        /// <summary>
        ///     Class constructor. Initialize the shared sharedPriceConsumerService
        ///     with the current group id and stock prices dictionary
        /// </summary>
        public OrderServicePriceConsumer()
        {
            sharedPriceConsumerService = new SharedPriceConsumerService(CONSUMER_GROUP_ID);
        }

        /// <summary>
        ///     Start consuming the events (messages)
        /// </summary>
        /// <param name="stoppingToken">
        ///     The cancellation token
        /// </param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Create a new task on a separate thread to prevent blocking the main thread
            var consumeTask = Task.Run(() => sharedPriceConsumerService.StartConsuming(stoppingToken), stoppingToken);

            // Run the consume task until cancellation is requested
            await Task.WhenAny(consumeTask, Task.Delay(Timeout.Infinite, stoppingToken));
        }

        /// <summary>
        ///     Stop consuming and dispose
        /// </summary>
        public override void Dispose()
        {
            sharedPriceConsumerService.StopConsuming();
            base.Dispose();
        }

        /// <summary>
        ///     Try to find the stock with the provided ticker and return the latest price
        /// </summary>
        /// <param name="ticker">
        ///     The stock's ticker
        /// </param>
        public ServiceActionResult<decimal> GetStockPrice(string ticker) {

            if (sharedPriceConsumerService.stockPrices.TryGetValue(ticker, out decimal value)) {
                return new ServiceActionResult<decimal>(HttpStatusCode.OK, "", value);
            }

            return new ServiceActionResult<decimal>(HttpStatusCode.NotFound, STOCK_NOT_FOUND, 0);
        }
    }
}
