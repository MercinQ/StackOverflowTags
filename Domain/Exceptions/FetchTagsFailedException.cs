namespace Domain.Exceptions
{
    public class FetchTagsFailedException : Exception
    {
        public FetchTagsFailedException()
        {
        }

        public FetchTagsFailedException(string message)
            : base(message)
        {
        }

        public FetchTagsFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
