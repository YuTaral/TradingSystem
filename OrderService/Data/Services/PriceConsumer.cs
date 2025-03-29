using Shared.Models;
using Shared.Utils;
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
        private readonly SharedPriceConsumer priceConsumer;

        /// <summary>
        ///     Class constructor. Initialize the shared sharedPriceConsumerService
        ///     with the current group id and stock prices dictionary
        /// </summary>
        public OrderServicePriceConsumer()
        {
            priceConsumer = new SharedPriceConsumer(CONSUMER_GROUP_ID);
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
            var consumeTask = Task.Run(() => priceConsumer.StartConsuming(stoppingToken), stoppingToken);

            // Run the consume task until cancellation is requested
            await Task.WhenAny(consumeTask, Task.Delay(Timeout.Infinite, stoppingToken));
        }

        /// <summary>
        ///     Stop consuming and dispose
        /// </summary>
        public override void Dispose()
        {
            priceConsumer.StopConsuming();
            base.Dispose();
        }

        /// <summary>
        ///     Return the latest stock price if stock with this ticker exists.
        ///     If the stock does not exist, return -1
        /// </summary>
        /// <param name="ticker">
        ///     The stock ticker
        /// </param>
        public decimal GetStockPrice(string ticker) {

            return priceConsumer.GetStockPrice(ticker);
        }
    }
}
