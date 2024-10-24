using System.ComponentModel.DataAnnotations;

namespace Shopee.Application.DTOs
{
    public class RegisterUserDTO
    {
        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string? Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}