using Shopee.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Services.Interfaces;
public interface IUserServices
{
    Task<UserResponseDto> RegisterAsync(UserRegisterRequestDto request);
    Task<CurrentUserResponseDto> GetCurrentUserAsync();
    Task<UserResponseDto> GetByIdAsync(Guid id);
    Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserRequestDto request);
    Task DeleteAsync(Guid id);
    Task<string> RevokeRefreshToken(RefreshTokenRequestDto refreshTokenRemoveRequest);
    Task<CurrentUserResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);

    Task<UserResponseDto> LoginAsync(UserLoginRequestDto request);
}