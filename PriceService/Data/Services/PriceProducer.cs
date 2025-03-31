    using Confluent.Kafka;
using System.Collections.Concurrent;
using System.Text.Json;
using static Shared.Constants;

namespace PriceService.Data.Services
{
    /// <summary>
    ///     PriceService - generate initial stocks and prices. 
    ///     Inherit BackgroundService and refresh the stock prices
    ///     on every one second.
    ///     Store the stocks in ConcurrentDictionary which is thread safe
    ///     and prevents race conditions
    /// </summary>
    public class PriceService : BackgroundService
    {
        private const int PRICE_REFRESH_MILLIS = 1000;

        private readonly ConcurrentDictionary<string, decimal> stockPrices = new();
        private readonly IProducer<string, string> kafkaProducer;
        private readonly Random random = new();

        /// <summary>
        ///     Class constructor
        /// </summary>
        public PriceService()
        {
            // Configure kafka
            var config = new ProducerConfig
            {
                BootstrapServers = KAFKA_BOOSTRAP_SERVER
            };

            kafkaProducer = new ProducerBuilder<string, string>(config).Build();

            PopulateStocks();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Generate the new prices in the range 0 - 100 and round to 2nd decimal
                    foreach (var s in stockPrices.Keys.ToList())
                    {
                        stockPrices[s] = Math.Round((decimal)(random.NextDouble() * 100), 2);
                    }

                    await kafkaProducer.ProduceAsync(KAFKA_STOCK_PRICE_UPDATE_TOPIC, new Message<string, string>
                    {
                        Key = KAFKA_STOCK_PRICE_UPDATE_KEY,
                        Value = JsonSerializer.Serialize(stockPrices)
                    }, stoppingToken);

                    // Schedule new refresh
                    await Task.Delay(PRICE_REFRESH_MILLIS, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Replace this with logging mechanism
                    Console.WriteLine($"Error in price update loop: {ex.Message}");
                }
            }
        }

        /// <summary>
        ///     Populat the stocks dictionary
        /// </summary>
        private void PopulateStocks() 
        {
            stockPrices["AAPL"] = 0;
            stockPrices["TSLA"] = 0;
            stockPrices["NVDA"] = 0;
            stockPrices["MFST"] = 0;
            stockPrices["AMZN"] = 0;
        }

        public override void Dispose()
        {
            // In case of system shutdown, make sure all messages are sent
            // and the resources have been disposed
            kafkaProducer.Flush(TimeSpan.FromSeconds(5));
            kafkaProducer.Dispose();
        }
    }
}
