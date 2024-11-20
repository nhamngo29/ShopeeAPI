using MediatR;
using Shopee.Application.DTOs;

namespace Shopee.Application.Commands.Auth
{
    public class SignOutCommand : IRequest<ApiReponse<string>>
    {
    }

    public class SignOutCommandHandler(IIdentityService identityService, ICurrentUser currentUser, ICookieService cookieService) : IRequestHandler<SignOutCommand, ApiReponse<string>>
    {
        public async Task<ApiReponse<string>> Handle(SignOutCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUser.GetCurrentUserId();//lấy user id từ
            var user = await identityService.GetRefreshTokenByIdUser(userId.ToString());
            user.RefreshToken = null;
            await identityService.SaveRefreshTokenUser(user);//save refresh token
            cookieService.Delete();
            return new ApiReponse<string>()
            {
                Message = "Đăng xuất thành công"
            };
        }
    }
}