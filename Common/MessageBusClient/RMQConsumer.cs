using System.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RMQMessageBusClient
{

    public class RMQConsumer<TMessage> : RMQClientBase
    {
        private bool _initialized;
        private readonly object _lockHandleForStartStop = new Object();
        private CancellationTokenSource _cancellationToken;
        private EventingBasicConsumer _consumer;
        private readonly string _name;
        private string _consumerTag;

        protected override RMQConsumerConfig Config => (RMQConsumerConfig)base.Config;

        public delegate void OnMessageReceivedEventHandler(TMessage message, string type, string messageId);

        public event OnMessageReceivedEventHandler OnMessageReceived;

        public RMQConsumer(RMQConsumerConfig config) : base(config)
        {
            _cancellationToken = new CancellationTokenSource();
            _name =
                $"RMQConsumer[{config.QueueName}][{typeof(TMessage).Name}][{Process.GetCurrentProcess().Id}]";
            State = RMQClientStateEnum.NotStarted;
        }

        private TMessage Deserialize(byte[] dequeuedItemBody)
        {
            if (dequeuedItemBody == null || dequeuedItemBody.Length < 1) return default;
            var bodyStr = System.Text.Encoding.UTF8.GetString(dequeuedItemBody);
            if (typeof(TMessage).IsAssignableFrom(typeof(string))) return (TMessage)(object)bodyStr; //To Be Cached.
            if (typeof(TMessage).IsPrimitive)
            {
                return (TMessage)System.Convert.ChangeType(bodyStr, typeof(TMessage));

            }

            return System.Text.Json.JsonSerializer.Deserialize<TMessage>(bodyStr);
        }

        public void StartDequeue()
        {
            if (State != RMQClientStateEnum.NotStarted)
            {
                throw new Exception("AlreadyStarted");
            }

            lock (_lockHandleForStartStop)
            {
                if (State != RMQClientStateEnum.NotStarted)
                {
                    throw new Exception("AlreadyStarted");
                }

                State = RMQClientStateEnum.Started;

                Setup();
                Channel.BasicQos(0, Config.PrefetchCount, true);

                _consumer = new EventingBasicConsumer(this.Channel);
                EventHandler<BasicDeliverEventArgs> consumerOnReceived = (sender, dequeuedItem) =>
                {
                    var msg = Deserialize(dequeuedItem.Body.ToArray());
                    try
                    {
                        this.OnMessageReceived.Invoke(msg, dequeuedItem.BasicProperties.Type,
                            dequeuedItem.BasicProperties.MessageId);
                    }
                    catch (Exception ex)
                    {
                        //noop
                    }

                    Channel.BasicAck(dequeuedItem.DeliveryTag, false);
                };
                if (_cancellationToken.IsCancellationRequested) return;
                _consumer.Received += consumerOnReceived;
                _consumer.Registered += (sender, args) => { State = RMQClientStateEnum.Connected; };
                _consumer.Shutdown += (sender, args) => { State = RMQClientStateEnum.ShutDown; };
                _consumerTag = Channel.BasicConsume(_consumer, Config.QueueName, false, _name);
            }
        }


        public void StopDequeue()
        {
            try
            {
                if (State == RMQClientStateEnum.NotStarted) return;
                if (!string.IsNullOrWhiteSpace(_consumerTag))
                {
                    Channel.BasicCancel(_consumerTag);
                }
            }
            finally
            {
                State = RMQClientStateEnum.NotStarted;
            }
        }



    }

    
}