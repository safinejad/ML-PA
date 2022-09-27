using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace RMQMessageBusClient
{
    public class RMQPublisher: RMQClientBase
    {
        private readonly string _name;

        public RMQPublisher(RMQConfig config) : base(config)
        {
            _name =
                $"RMQPublisher[{config.QueueName}][{Process.GetCurrentProcess().Id}]";
        }

        public void Publish<TMessage>(TMessage message, string messageId = "")
        {

            if (!(Channel?.IsOpen ?? false)) Setup();
            if (!(Channel?.IsOpen ?? false))
            {
                throw new Exception("Cannot open Channel!");
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            int rmqFailureCount = 0;
            var props = Channel.CreateBasicProperties();
            props.MessageId = messageId;
            props.Type = typeof(TMessage).Name;
            props.AppId = _name;
            props.Persistent = true;
            var bytes = new ReadOnlyMemory<byte>(Serialize(message));
            if (!string.IsNullOrWhiteSpace(Config.Exchange))
            {
                foreach (var configBindingRoutingKey in Config.BindingRoutingKeys)
                {
                    Channel.BasicPublish(Config.Exchange, configBindingRoutingKey, props, bytes);
                }
            }
            else
            {
                Channel.BasicPublish(string.Empty, Config.QueueName, props, bytes);
            }

        }

        private byte[] Serialize(object message)
        {
            var msgJson = System.Text.Json.JsonSerializer.Serialize(message);
            return System.Text.Encoding.UTF8.GetBytes(msgJson);
        }
    }
}