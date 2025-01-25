using Application.Abstraction;
using Application.Services.Interfaces;
using Domain.Aggregates;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TagLoaderService : ITagLoaderService
    {
        private readonly ITagsRepository _tagsRepository;
        private readonly IStackOverflowClient _stackOverflowClient;
        private readonly TagAggregate _tagAggregate;
        private readonly ILogger<TagLoaderService> _logger;

        public TagLoaderService(ITagsRepository tagsRepository,
            IStackOverflowClient stackOverflowClient,
            ILogger<TagLoaderService> logger,
            int targetFetchCount = 1000)
        {
            _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
            _stackOverflowClient = stackOverflowClient ?? throw new ArgumentNullException(nameof(stackOverflowClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tagAggregate = new TagAggregate();
        }

        public async Task FetchAndStoreTagsAsync(int targetFetchCount, int pageSize)
        {
            bool checkIfAnyExists =  await _tagsRepository.AnyExistsAsync();

            if (checkIfAnyExists) {
                _logger.LogInformation("Existing tags found. Removing all existing tags.");
                await _tagsRepository.RemoveAllAsync().ConfigureAwait(false);
            }

            await FetchFromAPI(targetFetchCount, pageSize).ConfigureAwait(false);

            await _tagsRepository.SaveAsync(_tagAggregate).ConfigureAwait(true);
        }

        private async Task FetchFromAPI(int targetFetchCount, int pageSize)
        {
            int page = 1;
            int totalFetched = 0;

            try
            {
                _logger.LogInformation("Starting to fetch tags from the StackOverflow API.");

                while (totalFetched < targetFetchCount)
                {
                    _logger.LogInformation("Fetching page {Page} with page size {PageSize}.", page, pageSize);

                    var tagItems = await _stackOverflowClient.FetchTagsAsync(page, pageSize);

                    var tags = tagItems.Select(item => new Tag(item.Name, item.Count)).ToList();
                    totalFetched += tags.Count;
                    _tagAggregate.AddTags(tags);
                    page++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching tags.");
                throw new FetchTagsFailedException("An error occurred while fetching tags from the API.", ex);
            }
            _logger.LogInformation("Finished fetching tags. Total tags fetched: {TotalFetched}.", totalFetched);
        }

    }
}
