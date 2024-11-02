namespace Shopee.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; } 
        public IList<string> Roles { get; set; }
    }
}