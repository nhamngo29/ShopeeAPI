using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Exceptions;

public class CookieService(IHttpContextAccessor httpContextAccessor): ICookieService
{
    public void Set(string token) => httpContextAccessor.HttpContext?.Response.Cookies.Append("token_key", token, new CookieOptions
    {
        HttpOnly = true,
        SameSite = SameSiteMode.None,
        Secure = true,
        MaxAge = TimeSpan.FromMinutes(30)
    });

    public void Delete() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("token_key");

    public string Get()
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies["token_key"];
        return string.IsNullOrEmpty(token) ? throw UserException.UserUnauthorizedException() : token;
    }
}