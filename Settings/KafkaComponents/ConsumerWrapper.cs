using Common;
using Common.Interfaces;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using BasicIC_Setting.Common;
using BasicIC_Setting.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicIC_Setting.KafkaComponents
{
    public class ConsumerWrapper
    {
        private ConsumerConfig _consumerConfig;
        private static ILogger _logger = CommonFunc.CreateInstanceDJ<ILogger>();
        private static int max_degree_of_parallelism = int.Parse(ConfigManager.StaticGet(Constants.CONF_MAX_DEGREE_PARALLELISM));

        public ConsumerWrapper()
        {
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = ConfigManager.StaticGet(Constants.CONF_KAFKA_BOOSTRAP_SERVER),
                GroupId = ConfigManager.StaticGet(Constants.CONF_KAFKA_GROUP_ID),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        }

        public async Task StartGetMess(IEnumerable<string> topics)
        {
            try
            {
                // Init topic before subscribe if topic not exist
                using (var adminClient = new AdminClientBuilder(_consumerConfig).Build())
                {
                    Metadata metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(30));
                    List<string> topcisApp = metadata.Topics.Select(a => a.Topic).ToList();
                    List<TopicSpecification> topicsNotExist = topics.Where(topic => !topcisApp.Any(item => item == topic))
                        .Select(item => new TopicSpecification { Name = item })
                        .ToList();
                    if (topicsNotExist != null && topicsNotExist.Count > 0) await adminClient.CreateTopicsAsync(topicsNotExist);
                }

                using (var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build())
                {
                    consumer.Subscribe(topics);

                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) =>
                    {
                        e.Cancel = true; // prevent the process from terminating.
                        cts.Cancel();
                    };

                    try
                    {
                        ActionBlock<ConsumerData> _actionBlock = new ActionBlock<ConsumerData>(async p =>
                        {
                            HandleProducer handleProducer = new HandleProducer();
                            await handleProducer.Register(p);
                        },
                            new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = max_degree_of_parallelism });
                        while (true)
                        {
                            try
                            {
                                if (_actionBlock.InputCount <= 0)
                                {
                                    var cr = consumer.Consume(cts.Token);

                                    // Convert UTC to UTC+7
                                    DateTime timeStamp = cr.Message.Timestamp.UtcDateTime.AddHours(7);
                                    ConsumerData data = new ConsumerData(cr.Topic, cr.Message.Value, cr.TopicPartitionOffset, consumer);
                                    _actionBlock.Post(data);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex);
                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        consumer.Close();
                        _logger.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

    }

    public class ConsumerData
    {
        public ConsumerData(string topic, string mess, TopicPartitionOffset offset, IConsumer<string, string> cs)
        {
            this.topic = topic;
            this.mess = mess;
            this.cs = cs;
            this.offset = offset;
        }

        public string topic { get; set; }
        public string mess { get; set; }
        public IConsumer<string, string> cs { get; set; }
        public TopicPartitionOffset offset { get; set; }
    }
}