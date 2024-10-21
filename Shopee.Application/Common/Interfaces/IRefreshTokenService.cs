

using Shopee.Domain.Entities;

namespace Shopee.Application.Common.Interfaces
{ 
    public interface IRefreshTokenService
    {
        public Task RemoveRefreshTokenAsync(string token);
        public Task<RefreshToken?> GetStoredRefreshTokenAsync(string token);
        public Task SaveRefreshToken(RefreshToken refreshToken);
    }
}