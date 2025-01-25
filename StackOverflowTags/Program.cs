using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Application;
using Application.Services.Interfaces;
using Infrastructure.ApiClients;
using Application.Abstraction;
using StackOverflowTags.Middlewares;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
//    ServiceLifetime.Scoped);

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
   //var tagLoaderService = scope.ServiceProvider.GetRequiredService<ITagLoaderService>();
   //await tagLoaderService.FetchAndStoreTagsAsync(100, 100).ConfigureAwait(false);
}


app.Run();

