using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Auth;

namespace Shopee.Application.Commands.Auth;

public class RefreshCommand : IRequest<ApiReponse<RefreshTokenResponseDTO>>
{
    public string RefreshToken { get; set; }
}

public class RefreshCommandHandler(ITokenService tokenService, IIdentityService identityService, ICurrentUser cookieService) : IRequestHandler<RefreshCommand, ApiReponse<RefreshTokenResponseDTO>>
{
    public async Task<ApiReponse<RefreshTokenResponseDTO>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var userId = cookieService.GetCurrentUserId();//lấy user id từ
        var user = await identityService.GetRefreshTokenByIdUser(userId);
        if (user == null || user.RefreshToken == null || user.RefreshTokenExpiry <= DateTime.Now || !user.RefreshToken.Equals(request.RefreshToken))//Kiểm tra refresh token có hợp lệ hay không
            throw UserException.UserUnauthorizedException();
        var roleUsers = await identityService.GetUserRolesAsync(user.Id);//get role user
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newToken = tokenService.GenerateJWTToken((user.Id, user.UserName, roleUsers, user.Email, user.FullName));
        user.RefreshToken = newRefreshToken;
        await identityService.SaveRefreshTokenUser(user);//save refresh token
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