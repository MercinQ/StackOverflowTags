using Application.Abstraction;
using Application.Dtos;
using Application.Dtos.Enums;
using Application.Services.Interfaces;
using Domain.Aggregates;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services
{
    public class TagProviderService: ITagProviderService
    {
        private readonly ITagsRepository _tagsRepository;

        public TagProviderService(ITagsRepository tagsRepository)
        {
            _tagsRepository = tagsRepository ?? throw new ArgumentNullException(nameof(tagsRepository));
        }

        public async Task<PaginatedTagsResult> GetTagsAsync(TagFilter filter)
        {
            var tags = await _tagsRepository.GetTagsAsync(filter);
            var totalCount = await _tagsRepository.GetTotalCountAsync();

            var tagAggregate = new TagAggregate();
            tagAggregate.AddTags(tags);

            var tagsWithStatistics = CalculateTagsStatistics(tags);

            var sortedTags = SortTags(tagsWithStatistics, filter);

            return new PaginatedTagsResult
            {
                Tags = tagsWithStatistics.Select(m => new TagDto(m.Name, m.Percentage)),
                TotalCount = totalCount
            };
        }

        private IEnumerable<TagStatistics> CalculateTagsStatistics(IEnumerable<Tag> tags)
        {
            var tagAggregate = new TagAggregate();
            tagAggregate.AddTags(tags);

            return tagAggregate.CalculateStatistics();
        }

        private IEnumerable<TagStatistics> SortTags(IEnumerable<TagStatistics> tags, TagFilter filter)
        {
            return filter.SortBy switch
            {
                SortBy.Name => filter.SortOrder == SortOrder.Desc
                    ? tags.OrderByDescending(t => t.Name)
                    : tags.OrderBy(t => t.Name),
                SortBy.Percentage => filter.SortOrder == SortOrder.Desc
                    ? tags.OrderByDescending(t => t.Percentage)
                    : tags.OrderBy(t => t.Percentage),
                _ => throw new ArgumentException("Invalid sort option", nameof(filter.SortBy))
            };
        }
    }
}
