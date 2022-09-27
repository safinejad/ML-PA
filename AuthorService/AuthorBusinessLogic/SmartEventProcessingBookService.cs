using System.Reflection;
using System.Text.Json;
using AuthorContracts;
using AutoMapper;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RMQMessageBusClient;

namespace AuthorBusinessLogic
{
    public class SmartEventProcessingBookService : BackgroundService
    {
        private RMQConsumer<string> _consumer;
        private readonly IMapper _mapper;
        private readonly BookConsumerConfig _config;
        private readonly IServiceScopeFactory _scope;

        private Dictionary<string, (Type type, MethodInfo method)> AcceptingTypes = new();

        public SmartEventProcessingBookService(IOptions<BookConsumerConfig> consumerConfig, IMapper mapper,
            IServiceScopeFactory scope)
        {
            _scope = scope;
            _config = consumerConfig.Value;
            _mapper = mapper;
            var types = Assembly.GetAssembly(typeof(Author)).GetTypes()
                .Where(x => typeof(IConsumerDto).IsAssignableFrom(x));
            var methods = typeof(IAuthorConsumerService).GetMethods();
            foreach (var type in types)
            {
                var method = methods.FirstOrDefault(mtd =>
                {
                    var methodParams = mtd.GetParameters();
                    if (methodParams.Length == 1 && methodParams[0].ParameterType == type) return true;
                    return false;
                });
                if (method != null)
                {

                    AcceptingTypes.Add(type.Name, (type, method));
                }
            }

            if (AcceptingTypes.Any())
            {
                _config.BindingRoutingKeys = AcceptingTypes.Keys.ToArray();
            }
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _consumer = new RMQConsumer<string>(_config);
            _consumer.OnMessageReceived += _consumer_OnMessageReceived;
            _consumer.StartDequeue();
            return Task.CompletedTask;
        }

        private void _consumer_OnMessageReceived(string message, string type, string messageId)
        {

            if (string.IsNullOrWhiteSpace(type))
            {
                return;
            }

            if (!AcceptingTypes.TryGetValue(type, out var typeNMethod))
            {
                return;
            }

            var obj = JsonSerializer.Deserialize(message, typeNMethod.type);
            using var scope = _scope.CreateScope();
            var consumerService = scope.ServiceProvider.GetRequiredService<IAuthorConsumerService>();
            typeNMethod.method.Invoke(consumerService, new[] { obj });

        }
    }
}