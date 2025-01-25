using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITagLoaderService, TagLoaderService>();
            services.AddScoped<ITagProviderService, TagProviderService>();

            return services;
        }
    }
}
