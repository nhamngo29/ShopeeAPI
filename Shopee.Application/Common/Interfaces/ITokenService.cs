using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Common.Interfaces
{
    public interface ITokenService
    {
        //public string GenerateToken(string userName, string password);
        (string Token, DateTime Expiration) GenerateJWTToken((string userId, string userName, IList<string> roles,string email,string fullName) userDetails);
        (string RefreshToken, DateTime ExpirationRefreshToken) GenerateRefreshToken();
    }
}
