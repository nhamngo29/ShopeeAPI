﻿using Shopee.Application.Common.Exceptions;
using System.IdentityModel.Tokens.Jwt;

public class CurrentUser(ITokenService tokenService, ICookieService cookieService) : ICurrentUser
{
    public string GetCurrentUserId()
    {
        try
        {
            var jwtCookie = cookieService.Get();
            var token = tokenService.ValidateToken(jwtCookie);
            var userId = token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            return userId;
        }
        catch (Exception)
        {
            throw UserException.UserUnauthorizedException();
        }
    }
}