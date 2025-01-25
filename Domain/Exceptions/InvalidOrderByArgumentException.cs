namespace Domain.Exceptions
{

    public class InvalidOrderByArgumentException : BadRequestException
    {
        public InvalidOrderByArgumentException(string value)
            : base($"Invalid OrderBy value: {value}")
        {
        }
    }
}
