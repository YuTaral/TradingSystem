using Confluent.Kafka;
using SharedData.Models;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using static SharedData.Constants;

namespace Shared.PriceConsumer
{
    /// <summary>
    ///     Shared PriceConsumer used to consume kafka events (messages)
    ///     from the PriceService to update the stock prices
    /// </summary>
    public class SharedPriceConsumer
    {
        private readonly IConsumer<string, string> kafkaConsumer;
        private readonly string topic = KAFKA_STOCK_PRICE_UPDATE_TOPIC;

        private ConcurrentDictionary<string, decimal> stockPrices = new();

        /// <summary>
        ///     Class constructor
        /// </summary>
        /// <param name="groupId">
        ///     The configuration group id. Use different group ids
        ///     for each individual consumer which use the shared consumer
        ///     so each consumer consumes the events (messages) individually
        /// </param>
        public SharedPriceConsumer(string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = KAFKA_BOOSTRAP_SERVER,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            kafkaConsumer = new ConsumerBuilder<string, string>(config).Build();
        }

        /// <summary>
        ///     Start consuming by subscribing the kafa consumer to the topic
        ///     and update the stock prices each time new event(message) arrives
        /// </summary>
        /// <param name="stoppingToken">
        ///     The cancellation token
        /// </param>
        public async Task StartConsuming(CancellationToken stoppingToken)
        {
            kafkaConsumer.Subscribe(topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = kafkaConsumer.Consume(stoppingToken);
                    var updatedStockPrices = JsonSerializer.Deserialize<ConcurrentDictionary<string, decimal>>(result.Message.Value);

                    if (updatedStockPrices != null)
                    {
                        // Update the stock prices
                        stockPrices = updatedStockPrices;
                    }

                }
                catch (ConsumeException ex)
                {
                    // Replace this with logging mechanism
                    Console.WriteLine($"Error: {ex.Error.Reason}");
                }
            }
        }

        /// <summary>
        ///     Stop consuming by closing the kafka consumer
        /// </summary>
        public void StopConsuming()
        {
            kafkaConsumer.Close();
        }


        /// <summary>
        ///     Return the latest stock price if stock with this ticker exists.
        ///     If the stock does not exist, return -1
        /// </summary>
        /// <param name="ticker">
        ///     The stock ticker
        /// </param>
        public decimal GetStockPrice(string ticker)
        {
            if (stockPrices.TryGetValue(ticker, out decimal value))
            {
                return value;
            }

            return -1;
        }
    }
}
