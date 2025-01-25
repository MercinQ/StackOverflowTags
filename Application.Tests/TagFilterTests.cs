using Application.Dtos;
using Application.Dtos.Enums;
using Domain.Exceptions;
using Shouldly;

namespace Application.Tests
{
    public class TagFilterTests
    {
        private readonly TagFilter _exceptedTagFilter = new TagFilter(1, 20, "Name", "Asc");

        [Fact]
        public void Constructor_Should_SetPageCorrectly_When_ValidArgumentsProvided()
        {
            const int page = 1;
            var tagFilter = new TagFilter(page, 20, "Name", "Asc");

            tagFilter.Page.ShouldBe(page);
        }

        [Fact]
        public void Constructor_Should_SetPageSizeCorrectly_When_ValidArgumentsProvided()
        {
            const int pageSize = 20; 
            var tagFilter = new TagFilter(1, pageSize, "Name", "Asc");

            tagFilter.PageSize.ShouldBe(pageSize);
        }

        [Fact]
        public void Constructor_Should_SetSortByCorrectly_When_ValidArgumentsProvided()
        {
            const string name = "Name";
            var tagFilter = new TagFilter(1, 20, "Name", "Asc");

            tagFilter.SortBy.ShouldBe(SortBy.Name);
        }

        [Fact]
        public void Constructor_Should_SetSortOrderCorrectly_When_ValidArgumentsProvided()
        {
            const string orderBy = "Asc";
            var tagFilter = new TagFilter(1, 20, "Name", orderBy);

            tagFilter.SortOrder.ShouldBe(SortOrder.Asc); 
        }

        [Fact]
        public void Constructor_Should_ThrowInvalidPageArgumentException_When_PageIsZero()
        {
            var exception = () => new TagFilter(0, 20, "Name", "Asc");

            exception.ShouldThrow<InvalidPageArgumentException>();
        }

        [Fact]
        public void Constructor_Should_ThrowInvalidPageSizeArgumentException_When_PageSizeIsGreaterThan100()
        {
            var exception = () => new TagFilter(2, 101, "Name", "Asc");

            exception.ShouldThrow<InvalidPageSizeArgumentException>();

        }

        [Fact]
        public void Constructor_Should_ThrowInvalidPageSizeArgumentException_When_PageSizeIsZero()
        {
            var exception = () => new TagFilter(1, 0, "Name", "Asc");

            exception.ShouldThrow<InvalidPageSizeArgumentException>();
        }

        [Fact]
        public void Constructor_Should_ThrowInvalidSortByArgumentException_When_SortByIsInvalid()
        {
            var exception = () => new TagFilter(1, 100, "Invalid", "Asc");

            exception.ShouldThrow<InvalidSortByArgumentException>();
        }

        [Fact]
        public void Constructor_Should_ThrowInvalidOrderByArgumentException_When_SortOrderIsInvalid()
        {
            var exception = () => new TagFilter(1, 100, "Name", "Invalid");

            exception.ShouldThrow<InvalidOrderByArgumentException>();
        }
    }
}
