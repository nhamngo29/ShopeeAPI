﻿using Microsoft.AspNetCore.Identity;

namespace Shopee.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
