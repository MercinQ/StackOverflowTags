using Application.Abstraction;
using Application.Services;
using Application.Services.Interfaces;
using Infrastructure;
using Infrastructure.ApiClients;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace StackOverflowTags.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.Remove(services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)));

                services.Remove(services.SingleOrDefault(service =>
                    service.ServiceType == typeof(IDbContextFactory<ApplicationDbContext>)));

                services.AddDbContextFactory<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer("Server=localhost,1434;Database=StackOverflowTags_Test;User=sa;Password=Password123!;TrustServerCertificate=True;");
                });

                services.AddScoped<ITagLoaderService, TagLoaderService>();
                services.AddScoped<ITagProviderService, TagProviderService>();
                services.AddScoped<ITagsRepository, TagsRepository>();

            });
            builder.UseEnvironment("Development");
        }
    }

}