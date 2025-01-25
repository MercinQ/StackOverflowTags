using Application.Abstraction;
using Application.Services;
using Domain.Aggregates;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Application.Tests
{
    public class TagLoaderServiceTests
    {
        private readonly Mock<ITagsRepository> _tagsRepositoryMock;
        private readonly Mock<IStackOverflowClient> _stackOverflowClientMock;
        private readonly Mock<ILogger<TagLoaderService>> _loggerMock;
        private readonly TagLoaderService _tagLoaderService;

        public TagLoaderServiceTests()
        {
            _tagsRepositoryMock = new Mock<ITagsRepository>(MockBehavior.Strict);
            _stackOverflowClientMock = new Mock<IStackOverflowClient>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<TagLoaderService>>();

            _tagLoaderService = new TagLoaderService(
                _tagsRepositoryMock.Object,
                _stackOverflowClientMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task FetchAndStoreTagsAsync_Should_Call_TagsRepository_RemoveAllAsync_Once_When_TagsRepository_AnyExistsAsync_Returns_True()
        {
            const int targetFetchCount = 1000;
            const int pageSize = 100;

            var tagsMock = new List<Tag>
            {
                new Tag("c#", 100),
                new Tag("typescript", 200),
            };

            _tagsRepositoryMock
                .Setup(repo => repo.AnyExistsAsync())
                .ReturnsAsync(true);

            _tagsRepositoryMock
                .Setup(repo => repo.RemoveAllAsync())
                .Returns(Task.CompletedTask);

            _stackOverflowClientMock
                .Setup(client => client.FetchTagsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(tagsMock);

            _tagsRepositoryMock
                .Setup(repo => repo.SaveAsync(It.IsAny<TagAggregate>()))
                .Returns(Task.CompletedTask);


            await _tagLoaderService.FetchAndStoreTagsAsync(targetFetchCount, pageSize);

            _tagsRepositoryMock.Verify(repo => repo.RemoveAllAsync(), Times.Once);
        }

        [Fact]
        public async Task FetchAndStoreTagsAsync_Should_Not_Call_TagsRepository_RemoveAllAsync_When_TagsRepository_AnyExistsAsync_Returns_False()
        {
            const int targetFetchCount = 1000;
            const int pageSize = 100;

            var tagsMock = new List<Tag>
            {
                new Tag("c#", 100),
                new Tag("typescript", 200),
            };

            _tagsRepositoryMock
                .Setup(repo => repo.AnyExistsAsync())
                .ReturnsAsync(false);

            _stackOverflowClientMock
                .Setup(client => client.FetchTagsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(tagsMock);
  
            _tagsRepositoryMock
                .Setup(repo => repo.SaveAsync(It.IsAny<TagAggregate>()))
                .Returns(Task.CompletedTask);

            await _tagLoaderService.FetchAndStoreTagsAsync(targetFetchCount, pageSize);

            _tagsRepositoryMock.Verify(repo => repo.RemoveAllAsync(), Times.Never);
        }

        [Fact]
        public async Task FetchAndStoreTagsAsync_Should_ThrowFetchTagsFailedException_When_StackOverflowClient_ThrowsException()
        {
            const int targetFetchCount = 1000;
            const int pageSize = 100;

            _tagsRepositoryMock
                .Setup(repo => repo.AnyExistsAsync())
                .ReturnsAsync(false);

            _stackOverflowClientMock
                .Setup(client => client.FetchTagsAsync(It.IsAny<int>(), It.IsAny<int>())).Throws<Exception>();

            _tagsRepositoryMock
                .Setup(repo => repo.SaveAsync(It.IsAny<TagAggregate>()))
                .Returns(Task.CompletedTask);


            var exception = await Should.ThrowAsync<FetchTagsFailedException>(
                () => _tagLoaderService.FetchAndStoreTagsAsync(targetFetchCount, pageSize)
            );
        }


        [Fact]
        public async Task FetchAndStoreTagsAsync_Should_Call_StackOverflowClient_FetchTagsAsync_ExpectedTimes()
        {
            var tagsMock = new List<Tag>
            {
                new Tag("c#", 100),
                new Tag("typescript", 200),
                new Tag("react", 500),
                new Tag("Angular", 200),
                new Tag(".net", 2000),
            };
            int targetFetchCount = 50;
            int expectedCalls = 10;
            int pageSize = 5;

            _tagsRepositoryMock
                .Setup(repo => repo.AnyExistsAsync())
                .ReturnsAsync(false);

            _stackOverflowClientMock
                .Setup(client => client.FetchTagsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(tagsMock);

            _tagsRepositoryMock
                .Setup(repo => repo.SaveAsync(It.IsAny<TagAggregate>()))
                .Returns(Task.CompletedTask);


             await _tagLoaderService.FetchAndStoreTagsAsync(targetFetchCount, pageSize);

            _stackOverflowClientMock.Verify(client => client.FetchTagsAsync(It.IsAny<int>(), pageSize), Times.Exactly(expectedCalls));
        }

    }
}
