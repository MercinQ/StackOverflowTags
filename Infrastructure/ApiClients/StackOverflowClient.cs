using Application.Abstraction;
using Domain.Entities;
using Infrastructure.ApiClients.Responses;
using System.Net.Http.Json;

namespace Infrastructure.ApiClients
{
    public class StackOverflowClient : IStackOverflowClient
    {
        private readonly HttpClient _httpClient;

        public StackOverflowClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Tag>> FetchTagsAsync(int page, int pageSize)
        {
            var response = await _httpClient.GetFromJsonAsync<StackOverflowApiResponse>(
                $"tags?page={page}&pagesize={pageSize}&order=desc&sort=popular&site=stackoverflow");

            if (response?.Items == null)
                return Enumerable.Empty<Tag>();
            
            return response.Items.Select(item => new Tag(item.Name, item.Count));
        }
    }
}
