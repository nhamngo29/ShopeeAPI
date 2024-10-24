using Microsoft.AspNetCore.Identity;

namespace Shopee.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}