using Application.Dtos;

namespace Application.Services.Interfaces
{
    public interface ITagProviderService
    {
        Task<PaginatedTagsResult> GetTagsAsync(TagFilter filter);
    }
}
