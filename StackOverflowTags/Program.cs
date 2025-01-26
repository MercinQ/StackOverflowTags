using Application;
using Application.Abstraction;
using Application.Services.Interfaces;
using Infrastructure;
using Infrastructure.ApiClients;
using Microsoft.EntityFrameworkCore;
using StackOverflowTags.Middlewares;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });


        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices();

        builder.Services.AddHttpClient<IStackOverflowClient, StackOverflowClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.stackexchange.com/2.3/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "StackOvwerFlowTags");
        });


        var app = builder.Build();

        app.UseMiddleware<ErrorHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        //run mgirations
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        //run initial Tags
        using (var scope = app.Services.CreateScope())
        {
            var tagLoaderService = scope.ServiceProvider.GetRequiredService<ITagLoaderService>();
            await tagLoaderService.FetchAndStoreTagsAsync(1000, 100).ConfigureAwait(false);
        }

        await app.RunAsync();
    }
}
