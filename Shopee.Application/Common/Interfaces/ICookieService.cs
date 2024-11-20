namespace Shopee.Application.Common.Interfaces;

public interface ICookieService
{
    void Set(string token);

    void Delete();

    string? Get();
}