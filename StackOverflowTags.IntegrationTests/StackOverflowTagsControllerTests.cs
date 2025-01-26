using Application.Dtos;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace StackOverflowTags.IntegrationTests
{
    public class StackOverflowTagsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;

        public StackOverflowTagsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            var serviceProvider = factory.Services.CreateScope().ServiceProvider;
            _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task InitializeAsync()
        {
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetTags_Should_Return_Status200()
        {
            _dbContext.Add(new Tag("c#", 100));

            _dbContext.SaveChanges();

            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=10&sortBy=name&sortOrder=asc");
            var responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetTags_Should_Return_CorrectTotalCount()
        {
            AddTagsToDb();
            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=10&sortBy=name&sortOrder=asc");
            var responseContent = await response.Content.ReadAsStringAsync();

            var tags = JsonSerializer.Deserialize<PaginatedTagsResult>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
         
            tags.TotalCount.Should().Be(50);
        }

        [Fact]
        public async Task GetTags_Should_Return_CorrectTagsCount()
        {
            AddTagsToDb();
            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=10&sortBy=name&sortOrder=asc");
            var responseContent = await response.Content.ReadAsStringAsync();

            var tags = JsonSerializer.Deserialize<PaginatedTagsResult>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            tags.Tags.Count().Should().Be(10);
        }

        [Fact]
        public async Task GetTags_Should_Return_SortedTagsByName_InAscendingOrder_When_SortBy_Name_And_SortOrder_Asc()
        {
            AddTagsToDb();
            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=10&sortBy=name&sortOrder=asc");
            var responseContent = await response.Content.ReadAsStringAsync();

            var tags = JsonSerializer.Deserialize<PaginatedTagsResult>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var tagNames = tags.Tags.Select(t => t.Name).ToList();

            tagNames.Should().BeInAscendingOrder();
        }

        [Fact]
        public async Task GetTags_Should_Return_SortedTagsByPercentage_InDescendingOrder_When_SortBy_Name_And_SortOrder_Desc()
        {
            AddTagsToDb();
            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=10&sortBy=percentage&sortOrder=Desc");
            var responseContent = await response.Content.ReadAsStringAsync();

            var tags = JsonSerializer.Deserialize<PaginatedTagsResult>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var tagPercentages = tags.Tags.Select(t => t.Percentage).ToList();

            tagPercentages.Should().BeInDescendingOrder();
        }

        [Fact]
        public async Task GetTags_Should_Return_Status400_When_PageSizeIsTooBig()
        {
            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=101&sortBy=name&sortOrder=asc");
    
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetTags_Should_Return_Status400_When_SortByValueIsInvalid()
        {
            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=100&sortBy=invalid&sortOrder=asc");
 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private void AddTagsToDb()
        {
            for (int i = 1; i <= 50; i++)
            {
                _dbContext.Add(new Tag($"tag{i}", i * 10));
            }

            _dbContext.SaveChanges();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
