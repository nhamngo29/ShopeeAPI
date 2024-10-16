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
    public class TokenService : ITokenService
    {

        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _expiryMinutes;

        public TokenService(string key, string issueer, string audience, string expiryMinutes)
        {
            _key = key;
            _issuer = issueer;
            _audience = audience;
            _expiryMinutes = expiryMinutes;
        }

        public (string Token, DateTime Expiration) GenerateJWTToken((string userId, string userName, IList<string> roles,string email,string fullName) userDetails)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
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
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_expiryMinutes));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiration, // Set thời gian hết hạn cho token
                signingCredentials: signingCredentials
            );

            // Encode token thành chuỗi
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Trả về token cùng với thời gian hết hạn
            return (encodedToken, expiration);
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        //public (string Token, DateTime Expiration) RefreshToken(string refreshToken)
        //{
        //    // Kiểm tra tính hợp lệ của refresh token (có thể lưu trong cơ sở dữ liệu hoặc một danh sách tạm thời)
        //    if (!IsValidRefreshToken(refreshToken))
        //    {
        //        throw new SecurityTokenException("Invalid refresh token");
        //    }

        //    // Tạo userDetails từ thông tin của refresh token (ví dụ: userId, userName, roles)
        //    //var userDetails = GetUserDetailsFromRefreshToken(refreshToken); // Bạn sẽ cần triển khai phương thức này

        //    // Tạo JWT token mới
        //    return GenerateJWTToken(userDetails);
        //}
        private bool IsValidRefreshToken(string refreshToken)
        {
            // Kiểm tra tính hợp lệ của refresh token
            // Cần tùy chỉnh theo cách bạn lưu trữ và quản lý refresh tokens
            return true; // Thay thế bằng logic kiểm tra thực tế
        }
    }
}
