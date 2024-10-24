namespace Shopee.Application.Common.Exceptions
{
    public class HttpStatusException : Exception
    {
        public int StatusCode { get; }

        public HttpStatusException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}