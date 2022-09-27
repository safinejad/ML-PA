namespace RMQMessageBusClient;

public class RMQConfig
{
    public bool Durable { get; set; }
    public string Uri { get; set; }
    public string QueueName { get; set; }
    public string Exchange { get; set; }
    public string[] BindingRoutingKeys { get; set; }
}

public class RMQConsumerConfig : RMQConfig
{
    public ushort PrefetchCount { get; set; }
}