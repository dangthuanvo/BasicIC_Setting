using Common.Interfaces;
using Common.Models;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Common.Commons
{
    [DataContract]
    public class ResponseService<T> : BaseResponseService
    {
        [DataMember]
        public bool status { get; set; }
        [DataMember]
        public int service_code = Constants.SERVICE_CODE;
        [DataMember]
        public int error_code { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public T data { get; set; }
        public HttpStatusCode status_code { get; set; }

        private static ILogger _logger = new Logger();

        /// <summary>
        /// return response success with resource response
        /// </summary>
        /// <param name="data"></param>
        public ResponseService(T data)
        {
            this.status = true;
            this.message = string.Empty;
            this.data = data;
            this.status_code = HttpStatusCode.OK;
        }

        public ResponseService()
        {
            this.status = true;
            this.message = string.Empty;
        }

        /// <summary>
        /// return response error with message exception
        /// </summary>
        /// <param name="ex"></param>
        public ResponseService(Exception ex)
        {
            this.status = false;
            this.message = ex.Message;
            this.data = default;
            this.exception = ex;
            this.status_code = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// return response error with message
        /// </summary>
        /// <param name="ex"></param>
        public ResponseService(string mess)
        {
            this.status = false;
            this.message = mess;
            this.data = default;
        }

        /// <summary>
        ///  return custom response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public ResponseService(bool status, string message, T data)
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }

        public ResponseService<T> BadRequest(int errorCode = -1)
        {
            _logger.LogError(this.message);
            this.status_code = HttpStatusCode.BadRequest;
            this.status = false;
            this.error_code = errorCode != -1 ? errorCode : this.error_code;
            return this;
        }

        public ResponseService<T> Unauthorized()
        {
            _logger.LogError(this.message);
            this.status_code = HttpStatusCode.Unauthorized;
            this.status = false;
            return this;
        }
    }
}
