namespace Shopee.Application.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public IDictionary<string, string[]>? Errors { get; }

        public ConflictException() : base()
        {
        }

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(IDictionary<string, string[]> errors, string message) : base(message)
        {
            Errors = errors;
        }

        public ConflictException(IDictionary<string, string[]> errors) : base()
        {
            Errors = errors;
        }
    }
}