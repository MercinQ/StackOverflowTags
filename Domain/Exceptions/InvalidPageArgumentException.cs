namespace Domain.Exceptions
{
    public class InvalidPageArgumentException : BadRequestException
    {
        public InvalidPageArgumentException(int value)
            : base($"Page must be greater than 0. Passed value: {value}")
        {
        }
    }
}
