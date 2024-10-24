namespace Shopee.Application.DTOs
{
    public class ApiReponse<T> where T : class
    {
        public string? Message { get; set; } = null!;
        public T? Response { get; set; }
    }
}