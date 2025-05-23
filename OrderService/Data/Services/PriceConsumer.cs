﻿using Shared.PriceConsumer;

namespace OrderService.Data.Services
{
    /// <summary>
    ///     OrderServicePriceConsumer as background service.
    ///     Use the shared PriceConsumerService to access the stock prices
    /// </summary>
    public class PriceConsumer(SharedPriceConsumer pc) : BackgroundService
    {
        private readonly SharedPriceConsumer priceConsumer = pc;

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
