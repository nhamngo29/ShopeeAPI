namespace Shopee.Application.DTOs
{
    public class SigninResponseDTO
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresRefreshToken { get; set; }
    }
}