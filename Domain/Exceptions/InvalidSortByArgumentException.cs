namespace Domain.Exceptions
{
    public class InvalidSortByArgumentException : BadRequestException
    {

        public InvalidSortByArgumentException(string value)
            : base($"Invalid SortBy value: {value}")
        {
        }
    }
}
