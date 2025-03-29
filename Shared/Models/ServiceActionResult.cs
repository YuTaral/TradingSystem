using System.Net;

namespace Shared.Models
{
    /// <summary>
    ///     Common class used to return response from each
    ///     service action and create custom HTTP Response
    /// </summary>
    public class ServiceActionResult<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ServiceActionResult(HttpStatusCode CodeVal, string MessageVal, T? DataVal)
        {
            Code = (int)CodeVal;
            Message = MessageVal;
            Data = DataVal;
        }

        public ServiceActionResult(int CodeVal, string MessageVal, T? DataVal)
        {
            Code = CodeVal;
            Message = MessageVal;
            Data = DataVal;
        }
    }
}
