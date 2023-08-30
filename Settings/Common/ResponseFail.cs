using Common.Commons;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BasicIC_Setting.Common
{
    public class ResponseFail<T> : IHttpActionResult
    {
        private HttpStatusCode statusCode { get; set; }
        private string message { get; set; }
        private Exception exception { get; set; }
        private ResponseService<T> resService { get; set; }

        public ResponseFail() { }

        public ResponseFail(HttpStatusCode statusCode, string errorMessage) : this(statusCode, errorMessage, null)
        { }

        public ResponseFail(HttpStatusCode statusCode, string errorMessage, Exception exception)
        {
            this.statusCode = statusCode;
            this.message = errorMessage;
            this.exception = exception;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            StringBuilder stringBldr = new StringBuilder();
            response.Content = new StringContent(JsonConvert.SerializeObject(resService), Encoding.UTF8, "application/json");
            return Task.FromResult(response);
        }

        public ResponseFail<T> Error(ResponseService<T> resService)
        {
            this.statusCode = resService.status_code;
            this.message = resService.message;
            this.resService = resService;
            return this;
        }
    }
}