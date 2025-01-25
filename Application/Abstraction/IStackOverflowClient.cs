using Domain.Entities;

namespace Application.Abstraction
{
    public interface IStackOverflowClient
    {
        Task<IEnumerable<Tag>> FetchTagsAsync(int page, int pageSize);
    }
}
