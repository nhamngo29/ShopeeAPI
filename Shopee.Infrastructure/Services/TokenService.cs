using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Shopee.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Infrastructure.Services
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings=jwtSettings;
        }

        public (string Token, DateTime Expiration) GenerateJWTToken((string userId, string userName, IList<string> roles,string email,string fullName) userDetails)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var (userId, userName, roles,email, fullName) = userDetails;

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, userId),
                new Claim(ClaimTypes.Email,email),
                new Claim("fullName",fullName)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Xác định thời gian hết hạn token
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.AccessTokenExpirationMinutes)); 

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration, // Set thời gian hết hạn cho token
                signingCredentials: signingCredentials
            );

            // Encode token thành chuỗi
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Trả về token cùng với thời gian hết hạn
            return (encodedToken, expiration);
        }
        public (string RefreshToken, DateTime ExpirationRefreshToken) GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return (Convert.ToBase64String(randomNumber), DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpirationDays));
            }
        }
    }
}
