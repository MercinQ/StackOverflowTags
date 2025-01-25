using Application.Dtos.Enums;
using Domain.Exceptions;

namespace Application.Dtos
{
    public class TagFilter
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public SortBy SortBy { get; init; }
        public SortOrder SortOrder { get; init; }

        public TagFilter(int page, int pageSize, string sortBy, string sortOrder)
        {

            if (page <= 0)
                throw new InvalidPageArgumentException(page);

            if (pageSize <= 0 || pageSize > 100)
                throw new InvalidPageSizeArgumentException(pageSize);

            Page = page;
            PageSize = pageSize;

            if (!Enum.TryParse<SortBy>(sortBy, true, out var parsedSortBy))
                throw new InvalidSortByArgumentException(sortBy);
            SortBy = parsedSortBy;

            if (!Enum.TryParse<SortOrder>(sortOrder, true, out var parsedSortOrder))
                throw new InvalidOrderByArgumentException(sortOrder);
            SortOrder = parsedSortOrder;
        }

    }
}
