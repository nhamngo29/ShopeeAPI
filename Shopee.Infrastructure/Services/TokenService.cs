using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shopee.Application.Common;
using Shopee.Application.Services.Interfaces;
using Shopee.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shopee.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _secretKey;
    private readonly string? _validIssuer;
    private readonly string? _validAudience;
    private readonly double _expires;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SettingConfiguration _appSettings;
    private readonly ILogger<TokenService> _logger;
    public TokenService(SettingConfiguration appSetting, UserManager<ApplicationUser> userManager, ILogger<TokenService> logger)
    {
        _userManager = userManager;
        _logger = logger;
        _appSettings = appSetting;
        var jwtSettings = appSetting.Jwt;
        if (appSetting.Jwt == null || string.IsNullOrEmpty(appSetting.Jwt.Key))
        {
            throw new InvalidOperationException("JWT secret key is not configured.");
        }
        _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        _validIssuer = jwtSettings.Issuer;
        _validAudience = jwtSettings.Audience;
        _expires = jwtSettings.AccessTokenExpirationMinutes;
    }
    public async Task<string> GenerateToken(ApplicationUser user)
    {
        var singingCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
        var claims = await GetClaimsAsync(user);
        var tokenOptions = GenerateTokenOptions(singingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
    /// <summary>
    /// Generates the token options for the JWT token.
    /// </summary>
    /// <param name="signingCredentials">The signing credentials for the token.</param>
    /// <param name="claims">The claims to be included in the token.</param>
    /// <returns>The generated JWT token options.</returns>
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        return new JwtSecurityToken(
            
            issuer: _validIssuer,
            audience: _validAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: signingCredentials

        );
    }

    /// <summary>
    /// Gets the claims for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the claims are retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of claims.</returns>
    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user?.Id??string.Empty),
                new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
                new Claim("FullName", user?.FullName ?? string.Empty),
                new Claim("Gender", user?.Gender ?? string.Empty)
            };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    public ClaimsPrincipal? ValidateToken(string token)
    {
        IdentityModelEventSource.ShowPII = true;
        TokenValidationParameters validationParameters = new()
        {
            ValidIssuer = _appSettings.Jwt.Issuer,
            ValidAudience = _appSettings.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);

        return principal;
    }

}