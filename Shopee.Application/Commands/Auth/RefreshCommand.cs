using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Auth;

namespace Shopee.Application.Commands.Auth;
public class RefreshCommand : IRequest<ApiReponse<RefreshTokenResponseDTO>>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshCommandHandler(ITokenService tokenService, IIdentityService identityService, ICurrentUser cookieService) : IRequestHandler<RefreshCommand, ApiReponse<RefreshTokenResponseDTO>>
{
    public async Task<ApiReponse<RefreshTokenResponseDTO>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = cookieService.GetCurrentUserId();//lấy user id từ 
            if (userId == null)
                throw UserException.UserUnauthorizedException();
            (DateTime refreshTokenExpiry, string? refreshToken) =await identityService.GetRefreshTokenByIdUser(userId);
            if (refreshToken == null || refreshTokenExpiry <= DateTime.Now || !refreshToken.Equals(request.RefreshToken))
                throw UserException.UserUnauthorizedException();
            var newRefreshToken = tokenService.GenerateRefreshToken();
            var newToken = tokenService.GenerateJWTToken();
            return new ApiReponse<RefreshTokenResponseDTO>();
        }
        catch (Exception e)
        {
            throw UserException.UserUnauthorizedException();
        }
    }
}
