using BookContracts;
using FakeRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookBusinessLogic
{
    public static class Extensions
    {
        public static void AddBookServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<FakeRepo<Author>>(provider => new FakeRepo<Author>());
            services.AddSingleton<FakeRepo<Book>>(provider => new FakeRepo<Book>());
            services.AddScoped<IBookBusinessService, BookBusinessService>();
            services.Configure<AuthorConsumerConfig>(config =>
                configuration.GetSection("AuthorConsumerConfig").Bind(config));
            services.Configure<BookPublisherConfig>(config =>
                configuration.GetSection("BookPublisherConfig").Bind(config));
            services.AddHostedService<EventProcessingAuthorService>();
        }
    }
}
