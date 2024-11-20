using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Exceptions;

public class CookieService(IHttpContextAccessor httpContextAccessor) : ICookieService
{
    public void Set(string token)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            MaxAge = TimeSpan.FromMinutes(30)
        };
        httpContextAccessor.HttpContext?.Response.Cookies.Append("X-Access-Token", token, options);
    }

    public void Delete() => httpContextAccessor.HttpContext?.Response.Cookies.Delete("X-Access-Token");

    public string? Get()
    {
        return httpContextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
    }
}