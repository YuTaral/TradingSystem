using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using static Shared.Constants;

namespace OrderService
{
    /// <summary>
    ///     Class to send event in the channel and subscibe for the response 
    ///     after the event subscriber execute the necessary logic. 
    ///     Implemented with "Remote Procedure Calling" pattern and RabbitMQ
    /// </summary>
    public class RabbitMQClient : IAsyncDisposable
    {
        private ConcurrentDictionary<string, TaskCompletionSource<ServiceActionResult<string>>>? callbackMapper;
        private IConnectionFactory? connectionFactory;
        private IConnection? connection;
        private IChannel? channel;
        private string? replyQueueName;
        private AsyncEventingBasicConsumer? consumer;

        /// <summary>
        ///     Start the client
        /// </summary>
        public async Task StartAsync()
        {
            await Initialize();

            OnResponse();

            await channel!.BasicConsumeAsync(replyQueueName!, true, consumer!);
        }

        /// <summary>
        ///     Initialize class variables
        /// </summary>
        private async Task Initialize()
        {
            callbackMapper = new();
            connectionFactory = new ConnectionFactory { HostName = RPC_HOST };
            connection = await connectionFactory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            replyQueueName = queueDeclareResult.QueueName;

            consumer = new AsyncEventingBasicConsumer(channel);
        }

        /// <summary>
        ///     Execute the logic when response is available - consists of 
        ///     trying to match the event corrletaionId, removing the callback from the
        ///     callback mapper and marking the task as completed - the caller can check the result
        /// </summary>
        private void OnResponse()
        {
            consumer!.ReceivedAsync += (model, ea) =>
            {
                string? correlationId = ea.BasicProperties.CorrelationId;

                if (!string.IsNullOrEmpty(correlationId))
                {
                    if (callbackMapper!.TryRemove(correlationId, out var tcs))
                    {
                        var body = ea.Body.ToArray();
                        var response = JsonSerializer.Deserialize<ServiceActionResult<string>>(Encoding.UTF8.GetString(body));

                        if (response != null)
                        {
                            tcs.TrySetResult(response!);
                        }
                    }
                }

                return Task.CompletedTask;
            };
        }

        /// <summary>
        ///     Call async method - send the event with the specified message
        /// </summary>
        /// <param name="message">
        ///     The message (serialized DTO)
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token
        /// </param>
        public async Task<ServiceActionResult<string>> CallAsync(string message, CancellationToken cancellationToken = default)
        {
            // Generate correlation id so the callback mapper knows which to callback to execute
            // and set the replyQueueName
            string correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyQueueName
            };

            // Add the task to the callback mapper
            var tcs = new TaskCompletionSource<ServiceActionResult<string>>(TaskCreationOptions.RunContinuationsAsynchronously);
            callbackMapper!.TryAdd(correlationId, tcs);

            // Public th event in the channel
            await channel!.BasicPublishAsync(exchange: string.Empty, routingKey: RPC_QUEUE, mandatory: true, 
                basicProperties: props, body: Encoding.UTF8.GetBytes(message), cancellationToken: cancellationToken);

            using CancellationTokenRegistration ctr =
                cancellationToken.Register(() =>
                {
                    callbackMapper.TryRemove(correlationId, out _);
                    tcs.SetCanceled();
                });

            return await tcs.Task;
        }

        /// <summary>
        ///     Dispose the channel and conenction
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await channel!.CloseAsync();
            await connection!.CloseAsync();
        }
    }
}
