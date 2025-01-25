using Application.Dtos;
using Domain.Aggregates;
using Domain.Entities;

namespace Application.Abstraction
{
    public interface ITagsRepository
    {
        Task SaveAsync(TagAggregate aggregate);
        Task RemoveAllAsync();
        Task<bool> AnyExistsAsync();
        Task<List<Tag>> GetTagsAsync(TagFilter filter);
        Task<int> GetTotalCountAsync();
    }
}
