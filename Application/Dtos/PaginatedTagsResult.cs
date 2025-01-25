namespace Application.Dtos
{
    public class PaginatedTagsResult
    {
        public IEnumerable<TagDto> Tags { get; set; }
        public int TotalCount { get; set; }
    }

    public record TagDto(string Name, double Percentage);
}
