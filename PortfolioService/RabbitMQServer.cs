using PortfolioService.Data;
using PortfolioService.Data.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using Shared.Models.DTO;
using System.Net;
using System.Text;
using System.Text.Json;
using static Shared.Constants;

namespace PortfolioService
{
    /// <summary>
    ///     Class to subscribe for events and trigger response when the logic is executed
    ///     Implemented with "Remote Procedure Calling" pattern and RabbitMQ.
    /// </summary>
    public class RabbitMQServer(IServiceScopeFactory sf)
    {
        private readonly IServiceScopeFactory scopeFactory = sf;
        private ConnectionFactory? factory;
        private IConnection? connection;
        private IChannel? channel;
        private AsyncEventingBasicConsumer? consumer;

        /// <summary>
        ///     Start the server
        /// </summary>
        public async Task StartAsync()
        {
            await Initialize();
           
            OnEvent();

            await channel!.BasicConsumeAsync(RPC_QUEUE, false, consumer!);
        }

        /// <summary>
        ///     Initialize class variables
        /// </summary>
        private async Task Initialize()
        {
            factory = new ConnectionFactory { HostName = RPC_HOST };
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel!.QueueDeclareAsync(queue: RPC_QUEUE, durable: false, exclusive: false,
               autoDelete: false, arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            consumer = new AsyncEventingBasicConsumer(channel);
        }

        /// <summary>
        ///     Execute the logic when event occurs - call the service method
        ///     and publish the response
        /// </summary>
        private void OnEvent()
        {
            consumer!.ReceivedAsync += async (sender, ea) =>
            {
                AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer) sender;
                IChannel ch = cons.Channel;
                string response = "";

                byte[] body = ea.Body.ToArray();
                IReadOnlyBasicProperties props = ea.BasicProperties;

                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId
                };

                try
                {
                    response = await ProcessEvent(Encoding.UTF8.GetString(body));
                }
                catch (Exception e)
                {
                    response = JsonSerializer.Serialize(new ServiceActionResult<string>(HttpStatusCode.BadRequest, e.Message, []));
                }
                finally
                {
                    // Publish the event when th result is available
                    var responseBytes = Encoding.UTF8.GetBytes(response);

                    await ch.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);

                    await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };
        }

        /// <summary>
        ///     Process the event by executing the exact method we need based on the provided message
        /// </summary>
        /// <param name="message">
        ///     The event message
        /// </param>
        private async Task<string> ProcessEvent(string message)
        {
            // As RabbitMQServer class is defined as singelton, we cannot directly use DI to import services
            // which depend on DBContext, use the factory to get the required service
            var scope = scopeFactory.CreateScope();

            // Try to deserialize the body
            var rPCEvent = JsonSerializer.Deserialize<RPCEvent>(message);

            if (rPCEvent == null)
            {
                return JsonSerializer.Serialize(new ServiceActionResult<string>(HttpStatusCode.BadRequest, UNEXPECTED_ERROR_OCCURRED, []));
            }
            else if (rPCEvent.EventType == RPCEventTypes.UPDATE_PORTFOLIO.ToString())
            {
                // Process update portfolio event
                var service = scope.ServiceProvider.GetRequiredService<IPortfolioService>();

                var orderDTO = JsonSerializer.Deserialize<OrderDTO>(rPCEvent.Message);

                if (orderDTO == null)
                {
                    return JsonSerializer.Serialize(new ServiceActionResult<string>(HttpStatusCode.BadRequest, UNEXPECTED_ERROR_OCCURRED, []));
                }

                var result = await service.UpdatePortfolio(orderDTO);
                return JsonSerializer.Serialize(result);
            }

            // Unknown event type
            return JsonSerializer.Serialize(new ServiceActionResult<string>(HttpStatusCode.BadRequest, UNEXPECTED_ERROR_OCCURRED, []));
        }
    }
}
