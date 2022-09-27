using System.Text.Json;
using AutoMapper;
using BookContracts;
using BookContracts.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RMQMessageBusClient;

namespace BookBusinessLogic
{
    public class EventProcessingAuthorService: BackgroundService
    {
        private RMQConsumer<string> _consumer;
        private readonly IMapper _mapper;
        private readonly AuthorConsumerConfig _config;
        private readonly IServiceScopeFactory _scope;

        public EventProcessingAuthorService(IOptions<AuthorConsumerConfig> consumerConfig, IMapper mapper, IServiceScopeFactory scope)
        {
            _scope = scope;
            _config = consumerConfig.Value;
            _mapper = mapper;
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

            var eventType = (MessageEventTypeEnum)Enum.Parse(typeof(MessageEventTypeEnum), type);
            switch (eventType)
            {
                case MessageEventTypeEnum.Save:
                    SaveAuthor(message);
                    break;
                case MessageEventTypeEnum.Delete:
                    var externalId = long.Parse(message);
                    DeleteAuthor(externalId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void DeleteAuthor(long externalId)
        {
            using var scope = _scope.CreateScope();
            var bookBusinessService = scope.ServiceProvider.GetRequiredService<IBookBusinessService>();
            bookBusinessService.DeleteAuthorByExternalId(externalId);
        }

        private void SaveAuthor(string message)
        {
            var authorDto = JsonSerializer.Deserialize<AuthorPublishSaveDto>(message);
            var author = _mapper.Map<Author>(authorDto);
            using var scope = _scope.CreateScope();
            var bookBusinessService = scope.ServiceProvider.GetRequiredService<IBookBusinessService>();
            bookBusinessService.SaveAuthor(author);
        }


    }
}