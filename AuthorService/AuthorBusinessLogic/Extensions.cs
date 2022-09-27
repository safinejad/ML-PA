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
            services.AddScoped<IAuthorBusinessService, AuthorBusinessService>();
            services.Configure<AuthorPublisherConfig>(config =>
                configuration.GetSection("AuthorPublisherConfig").Bind(config));
        }
    }
}
