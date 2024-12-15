using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Auth;
using Shopee.Application.Services.Interfaces;

namespace Shopee.Application.Commands.Auth;

public class RefreshCommand : IRequest<ApiReponse<RefreshTokenResponseDTO>>
{
    public string RefreshToken { get; set; }
}

public class RefreshCommandHandler(ITokenService tokenService, IIdentityService identityService, ICurrentUserService currentUserService,ICookieService cookieService) : IRequestHandler<RefreshCommand, ApiReponse<RefreshTokenResponseDTO>>
{
    public async Task<ApiReponse<RefreshTokenResponseDTO>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.GetUserByRefreshToken(request.RefreshToken);
        if (user == null || user.RefreshToken == null || !user.RefreshToken.Equals(request.RefreshToken))//Kiểm tra refresh token có hợp lệ hay không
            throw UserException.UserUnauthorizedException();
        if(user.RefreshTokenExpiry <= DateTime.Now)
            throw new HttpStatusException(498, "Refresh token has expired.");
        var roleUsers = await identityService.GetUserRolesAsync(user.Id);//get role user
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newToken = await tokenService.GenerateToken(user);
        user.RefreshToken = newRefreshToken;
        await identityService.SaveRefreshTokenUser(user);//save refresh token
        cookieService.Set(newToken);
        return new ApiReponse<RefreshTokenResponseDTO>
        {
            Message = "Refresh token",
            Response = new RefreshTokenResponseDTO()
            {
                Token = newToken,
                RefreshToken = newRefreshToken
            }
        };
    }
}