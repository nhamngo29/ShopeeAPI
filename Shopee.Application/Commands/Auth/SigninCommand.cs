using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.Services.Interfaces;

namespace Shopee.Application.Commands.Auth
{
    public class SignInCommand : IRequest<ApiReponse<AuthResponseDTO>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class SignInCommandHandler(ITokenService tokenService, IIdentityService identityService, ICookieService cookieService) : IRequestHandler<SignInCommand, ApiReponse<AuthResponseDTO>>
    {
        public async Task<ApiReponse<AuthResponseDTO>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var result = await identityService.SigninUserAsync(request.UserName, request.Password);

            if (!result)
            {
                throw new HttpStatusException(401, "Tên tài khoản hoặc mật khẩu không đúng, vui lòng thử lại");
            }
            var userId = await identityService.GetUserIdAsync(request.UserName);
            var user = await identityService.GetRefreshTokenByIdUser(userId);
            var roleUsers = await identityService.GetUserRolesAsync(user.Id);//get role user
            string token = await tokenService.GenerateToken(user);
            string refreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            await identityService.SaveRefreshTokenUser(user);//save refresh token
            cookieService.Set(token);
            return new ApiReponse<AuthResponseDTO>()
            {
                Message = "Đăng nhập thành công",
                Response = new AuthResponseDTO()
                {
                    Roles = roleUsers,
                    RefreshToken = refreshToken,
                    AccessToken = token
                }
            };
        }
    }
}