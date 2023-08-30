using Confluent.Kafka;
using System;

namespace Common.Commons
{
    public class ResponseMessage<T> : ResponseService<T>
    {
        public string topic { get; set; }
        public string tenant_id { get; set; }
        public string email { get; set; }
        public string token { get; set; }

        public ResponseMessage() : base() { }

        public ResponseMessage(string topic, Exception ex) : base(ex)
        {
            this.topic = topic;
        }

        public ResponseMessage(string topic, T data, string token = null, string tenant_id = null, string email = null) : base(data)
        {
            this.token = token;
            this.topic = topic;
            this.tenant_id = tenant_id;
            this.email = email;
        }

        /// <summary>
        /// return response error with message exception
        /// </summary>
        /// <param name="ex"></param>
        public ResponseMessage(string topic, KafkaException ex)
        {
            this.status = false;
            this.message = ex.Message;
            this.data = default;
            this.exception = ex;
            this.topic = topic;
        }
    }
}
