using System.Net;

namespace Common.Commons
{
    public class BaseResponse<T>
    {
        public HttpStatusCode statusCode { get; set; }
        public string status { get; set; }
        public string mess { get; set; }
        public T data { get; set; }
        public bool success { get; set; }
    }
}
