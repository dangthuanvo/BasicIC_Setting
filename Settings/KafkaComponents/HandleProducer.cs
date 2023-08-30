using Common;
using Common.Models;
using Confluent.Kafka;
using BasicIC_Setting.Config;
using BasicIC_Setting.Models.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIC_Setting.KafkaComponents
{
    public class HandleProducer
    {
        private static int maxErrorConfig = int.Parse(ConfigManager.StaticGet(Constants.CONF_MAX_ERROR_MESS));
        private BaseResponseService response;
        private List<TopicError> errors = new List<TopicError>();

        public HandleProducer()
        {
        }
        /// <summary>
        /// register handle topic
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="mess"></param>
        /// <returns></returns>
        public async Task Register(ConsumerData data)
        {
            response = new BaseResponseService();

            switch (data.topic)
            {
            }

            //handle commit
            if (response.ready_commit)
            {
                data.cs.Commit();
            }
            else
            {
                //UpdateError(data.topic);
                //if (!isValidTopicError(data.topic))
                //{
                //    var topics = data.cs.Subscription;
                //    topics.Remove(data.topic);
                //    data.cs.Subscribe(topics);
                //}
            }

            //handle commit
            if (response.ready_commit)
            {
                IEnumerable<TopicPartitionOffset> offets = new List<TopicPartitionOffset>() { data.offset };
                data.cs.Commit(offets);
            }
        }

        #region logic handle
        /// <summary>
        /// update error topic
        /// </summary>
        /// <param name="topic"></param>
        private void UpdateError(string topic, bool reset_error = false)
        {
            if (!(errors.Any(item => item.topic_name == topic)))
            {
                errors.Add(new TopicError { topic_name = topic, max_error = 1, total_error = 1 });
            }
            else
            {
                foreach (var item in errors)
                {
                    if (item.topic_name == topic)
                    {
                        item.max_error++;
                        item.total_error++;
                    }
                }
            }
        }

        /// <summary>
        /// number topic error can accepted
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        private bool isValidTopicError(string topic)
        {
            TopicError topicError = errors.Find(item => item.topic_name == topic);
            if (topicError != null && topicError.max_error >= maxErrorConfig) return false;
            return true;
        }
        #endregion
    }
}