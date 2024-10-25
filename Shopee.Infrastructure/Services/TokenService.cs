using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shopee.Application.Common;
using Shopee.Application.Common.Interfaces;
using Shopee.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shopee.Infrastructure.Services;

public class TokenService(SettingConfiguration appSettings) : ITokenService
{
    private readonly ApplicationDbContext _context;

    public string GenerateJWTToken((string userId, string userName, IList<string> roles, string email, string fullName) userDetails)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var (userId, userName, roles, email, fullName) = userDetails;

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, userId),
            new Claim(ClaimTypes.Email,email),
            new Claim("fullName",fullName)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Xác định thời gian hết hạn token
        var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(appSettings.Jwt.AccessTokenExpirationMinutes));

        var token = new JwtSecurityToken(
            issuer: appSettings.Jwt.Issuer,
            audience: appSettings.Jwt.Audience,
            claims: claims,
            expires: expiration, // Set thời gian hết hạn cho token
            signingCredentials: signingCredentials
        );

        // Encode token thành chuỗi
        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Trả về token cùng với thời gian hết hạn
        return encodedToken;
    }

    public  string GenerateRefreshToken()
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
            ValidIssuer = appSettings.Jwt.Issuer,
            ValidAudience = appSettings.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);

        return principal;
    }
}