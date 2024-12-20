﻿using Microsoft.AspNetCore.Identity;

namespace Shopee.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? BirthOfDay { get; set; }
        public string? Avatar { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string? Gender { get; set; }
        public DateTime UpdateAt { get; private set; }= DateTime.Now;
        public DateTime CreateAt { get; private set; } = DateTime.Now;
    }
}