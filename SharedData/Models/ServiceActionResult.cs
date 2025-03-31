using System.Net;
using System.Text.Json.Serialization;

namespace SharedData.Models
{
    /// <summary>
    ///     Common class used to return response from each
    ///     service action
    /// </summary>
    public class ServiceActionResult<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<T> Data { get; set; }

        public ServiceActionResult(HttpStatusCode CodeVal, string MessageVal, List<T> DataVal)
        {
            Code = (int)CodeVal;
            Message = MessageVal;
            Data = DataVal;
        }

        [JsonConstructor]
        public ServiceActionResult(int Code, string Message, List<T> Data)
        {
            this.Code = Code;
            this.Message = Message;
            this.Data = Data;
        }
    }
}
