namespace Application.Services.Interfaces
{
    public interface ITagLoaderService
    {
        Task FetchAndStoreTagsAsync(int targetFetchCount, int pageSize);
    }
}
