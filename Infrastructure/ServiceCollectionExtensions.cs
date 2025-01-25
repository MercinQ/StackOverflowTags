using Application.Abstraction;
using Infrastructure.ApiClients;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ITagsRepository, TagsRepository>();
            services.AddScoped<IStackOverflowClient, StackOverflowClient>();
            return services;
        }
    }
}
