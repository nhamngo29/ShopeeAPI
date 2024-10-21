using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Interfaces;
using Shopee.Domain.Entities;
using Shopee.Infrastructure.Data;

namespace Shopee.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveRefreshToken(RefreshToken refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.IdUser == refreshToken.IdUser);
            if (token != null)
            {
                token.Token = refreshToken.Token;
                token.ExpirationDate = DateTime.UtcNow;
                _context.RefreshTokens.Update((RefreshToken)token);
                await _context.SaveChangesAsync();
            }
            else
            {
                var rt=new RefreshToken(refreshToken.IdUser, refreshToken.Token, refreshToken.ExpirationDate);
                await _context.RefreshTokens.AddAsync(rt);
                await _context.SaveChangesAsync();
            }    
        }
        public async Task<RefreshToken?> GetStoredRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(u => u.Token == token);
        }
        public async Task RemoveRefreshTokenAsync(string token)
        {
            var refreshToken = await GetStoredRefreshTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.Token = null;
                _context.RefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
            }

        }
    }
}