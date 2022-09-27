using AuthorContracts;
using FakeRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorBusinessLogic
{
    public static class Extensions
    {
        public static void AddAuthorServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<FakeRepo<Author>>(provider => new FakeRepo<Author>());
            services.AddSingleton<FakeRepo<Book>>(provider => new FakeRepo<Book>());
            services.AddScoped<IAuthorBusinessService, AuthorBusinessService>();
            services.AddScoped<IAuthorConsumerService, AuthorConsumerService>();
            services.Configure<AuthorPublisherConfig>(config =>
                configuration.GetSection("AuthorPublisherConfig").Bind(config));
            services.Configure<BookConsumerConfig>(config =>
                configuration.GetSection("BookConsumerConfig").Bind(config));
            services.AddHostedService<SmartEventProcessingBookService>();
        }
    }
}
