using RabbitMQ.Client;

namespace RMQMessageBusClient;

public abstract class RMQClientBase : IDisposable
{
    public delegate void OnStateChangingEventHandler(RMQClientStateEnum state, RMQClientBase source);
    public event OnStateChangingEventHandler OnStateChanging;

    public RMQClientStateEnum State
    {
        get => _state;
        protected set
        {
            try
            {
                if (_state != value)
                {
                    OnStateChanging?.Invoke(value, this);
                }
            }
            finally
            {
                _state = value;
            }
        }
    }

    protected virtual RMQConfig Config { get; private set; }
    protected IConnection Connection { get; private set; }
    private ConnectionFactory _factory;
    private bool _declared;
    private RMQClientStateEnum _state;
    protected IModel Channel { get; private set; }

    public RMQClientBase(RMQConfig config)
    {
        Config = config;
        _declared = false;
    }

    private const int WaitTime = 60 * 1000;
    private void CreateConnection()
    {
        if (Connection == null || !Connection.IsOpen)
        {
            try
            {
                Channel?.Dispose();
            }
            catch (Exception ex)
            {
                //noop
            }

            Channel = null;
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(Config.Uri),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
                TopologyRecoveryEnabled = true
            };
            Connection = factory.CreateConnection();
        }

        if (Channel == null || !Channel.IsOpen)
        {
            try
            {
                Channel?.Dispose();
            }
            catch (Exception ex)
            {
                //noop
            }
            Channel = Connection.CreateModel();
        }
    }

    private void Declare()

    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Config.QueueName))
            {
                var retryQueueName = Config.QueueName + ".Retry";
                var workExchange = Config.QueueName + ".WorkEx";
                var retryExchange = Config.QueueName + ".RetryEx";
                Channel.ExchangeDeclare(workExchange, "direct", Config.Durable, false);
                Channel.ExchangeDeclare(retryExchange, "direct", Config.Durable, false);


                Channel.QueueDeclare(Config.QueueName, Config.Durable, false, false,
                    new Dictionary<string, object>
                    {
                        {
                            "x-dead-letter-queue", retryQueueName
                        }
                    });
                Channel.QueueBind(Config.QueueName, workExchange, Config.QueueName);
                Channel.QueueDeclare(retryQueueName, Config.Durable, false, false,
                    new Dictionary<string, object>
                    {
                        {
                            "x-dead-letter-queue", Config.QueueName
                        },
                        {
                            "x-message-ttl", WaitTime //time to delay for unhandled dead messages
                        }
                    });
                Channel.QueueBind(retryQueueName, retryExchange, retryQueueName);
            }


            if (!string.IsNullOrWhiteSpace(Config.Exchange))
            {
                Channel.ExchangeDeclare(Config.Exchange, "topic", Config.Durable, false);

                if (!string.IsNullOrWhiteSpace(Config.QueueName) &&
                    (Config.BindingRoutingKeys?.Length ?? 0) > 0)
                {
                    foreach (var routingKey in Config.BindingRoutingKeys)
                    {
                        Channel.QueueBind(Config.QueueName, Config.Exchange, routingKey);
                    }
                }
            }
        }
        finally
        {
            _declared = true;
        }
    }

    protected void Setup()
    {
        CreateConnection();
        if (!_declared)
        {
            Declare();
        }
    }

    public void Dispose()
    {
        Connection.Dispose();
        Channel.Dispose();
    }
}