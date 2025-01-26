using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StackOverflowTags.Integration.Tests.Data;
using System.Net.Http.Json;
using System.Net;
using StackOverflowTags.Integration.Tests.Data.Entitites;

namespace StackOverflowTags.Integration.Tests
{
    public class StackOverflowTagsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;

        public StackOverflowTagsIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            var serviceProvider = factory.Services.CreateScope().ServiceProvider;
            _dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task InitializeAsync()
        {
            // Clean database before each test
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            await _dbContext.SaveChangesAsync();
        }

        //check if specific tags that are in the databse are returned
        [Fact]
        public async Task GetTags_ReturnsTags()
        {
            // Seed test data
            _dbContext.Tags.Add(new Tag { Id = Guid.NewGuid(), Name = "C#",  Count = 100});
            _dbContext.Tags.Add(new Tag { Id = Guid.NewGuid(), Name = "F#",  Count = 12});
            _dbContext.Tags.Add(new Tag { Id = Guid.NewGuid(), Name = ".NET",  Count = 1000});
            _dbContext.SaveChanges();

            var response = await _client.GetAsync("/api/StackOverflowTags?page=1&pageSize=10&sortBy=name&sortOrder=asc");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var tags = await response.Content.ReadFromJsonAsync<IEnumerable<Tag>>();
            tags.Should().ContainSingle(tag => tag.Name == "C#");
        }
    }
}
