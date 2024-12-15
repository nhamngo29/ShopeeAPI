using Shopee.Domain.Entities;
using System.Security.Claims;

namespace Shopee.Application.Services.Interfaces;
public interface ITokenService
{
    Task<string> GenerateToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
