using Domain.Aggregates;
using Domain.Entities;
using Shouldly;

namespace Domain.Tests
{
    public class TagAggregateTests
    {
        [Fact]
        public void AddTags_Should_AddUniqueTagsOnly()
        {
            var tagAggregate = new TagAggregate();
            var tags = new List<Tag>
            {
                new Tag("Tag1", 10),
                new Tag("Tag2", 20),
                new Tag("Tag1", 15) //duplicate tag
            };

            tagAggregate.AddTags(tags);

            tagAggregate.Tags.Count.ShouldBe(2);
        }

        [Fact]
        public void AddTags_Should_HandleEmptyCollection()
        {
            var tagAggregate = new TagAggregate();

            tagAggregate.AddTags(new List<Tag>());

            tagAggregate.Tags.ShouldBeEmpty(); 
        }

        [Fact]
        public void CalculateStatistics_Should_CalculateCorrectPercentages()
        {
            var tagAggregate = new TagAggregate();
            var tags = new List<Tag>
            {
                new Tag("Tag1", 10),
                new Tag("Tag2", 30)
            };
            tagAggregate.AddTags(tags);

            var statistics = tagAggregate.CalculateStatistics().ToList();

            // Tag1 percentage: 10 / (10 + 30) * 100 = 25
            // Tag2 percentage: 30 / (10 + 30) * 100 = 75
            statistics.ShouldContain(stat => stat.Name == "Tag1" && stat.Percentage == 25);
            statistics.ShouldContain(stat => stat.Name == "Tag2" && stat.Percentage == 75);
        }

    }
}