using Domain.Entities;
using Domain.ValueObjects;
using System.Net.NetworkInformation;

namespace Domain.Aggregates
{
    public class TagAggregate
    {
        private readonly List<Tag> _tags = new();

        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        public void AddTags(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                bool anyDuplicate = _tags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase));
                if (anyDuplicate)
                    continue;

                _tags.Add(tag);
            }
        }

        public IEnumerable<TagStatistics> CalculateStatistics()
        {
            var totalCount = _tags.Sum(tag => tag.Count);
            return _tags.Select(tag =>
                new TagStatistics(tag.Name,
                Math.Round((double)tag.Count / totalCount * 100, 2)
                ));
        }

    }
}
