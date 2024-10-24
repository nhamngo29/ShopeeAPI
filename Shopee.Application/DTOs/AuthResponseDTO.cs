namespace Shopee.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; set; }
        public List<string> Roles { get; set; }
    }
}