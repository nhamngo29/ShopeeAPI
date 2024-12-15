using Shopee.Application.Common.Exceptions;
using Shopee.Application.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

public class CurrentUser(ITokenService tokenService, ICookieService cookieService) : ICurrentUser
{
    public Guid GetCurrentUserId()
    {
        try
        {
            var jwtCookie = cookieService.Get();
            if (jwtCookie != null)
            {
                var token = tokenService.ValidateToken(jwtCookie);
                var userIdClaim = token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                Guid.TryParse(userIdClaim, out var userId);
                return userId;


            }
            else
                return Guid.Empty;
        }
        catch (Exception)
        {
            throw UserException.UserUnauthorizedException();
        }
    }

}