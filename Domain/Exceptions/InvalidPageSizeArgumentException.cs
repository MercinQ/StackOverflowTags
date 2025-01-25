namespace Domain.Exceptions
{
    public class InvalidPageSizeArgumentException : BadRequestException
    {
        public InvalidPageSizeArgumentException(int value)
            : base($"PageSize must be between 1 and 100. Passed value: {value}")
        {
        }
    }
}
