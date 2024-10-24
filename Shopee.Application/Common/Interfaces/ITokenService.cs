namespace Shopee.Application.Common.Interfaces
{
    public interface ITokenService
    {
        //public string GenerateToken(string userName, string password);
        string GenerateJWTToken((string userId, string userName, IList<string> roles, string email, string fullName) userDetails);

        //string GenerateRefreshToken();
    }
}