using System.Text.Json;
using System.Text.Json.Serialization;
using static Shared.Constants;

namespace Shared.Models.DTO
{
    /// <summary>
    ///     Class used to send RPC event with RabbitMQ
    /// </summary>
    public class RPCEvent
    {
        public string EventType { get; set; }
         
        public string Message { get; set; }

        public RPCEvent(RPCEventTypes et, object m) {
            EventType = et.ToString();
            Message = JsonSerializer.Serialize(m);
        }

        [JsonConstructor]
        public RPCEvent(string EventType, string Message)
        {
            this.EventType = EventType;
            this.Message = Message;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
