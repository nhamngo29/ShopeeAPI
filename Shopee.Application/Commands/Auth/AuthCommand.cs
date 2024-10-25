using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;

namespace Shopee.Application.Commands.Auth
{
    public class AuthCommand : IRequest<ApiReponse<AuthResponseDTO>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class AuthCommandHandler : IRequestHandler<AuthCommand, ApiReponse<AuthResponseDTO>>
    {
        private readonly ITokenService _tokenService;
        private readonly IIdentityService _identityService;

        public AuthCommandHandler(IIdentityService identityService, ITokenService tokenGenerator)
        {
            _identityService = identityService;
            _tokenService = tokenGenerator;
        }

        public async Task<ApiReponse<AuthResponseDTO>> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.SigninUserAsync(request.UserName, request.Password);

            if (!result)
            {
                throw new HttpStatusException(401, "Tên tài khoản của bạn hoặc Mật khẩu không đúng, vui lòng thử lại");
            }

            var (userId, fullName, userName, email, roles) = await _identityService.GetUserDetailsAsync(await _identityService.GetUserIdAsync(request.UserName));

            string token = _tokenService.GenerateJWTToken((userId, userName, roles, email, fullName));

            return new ApiReponse<AuthResponseDTO>()
            {
                Message = "Đăng nhập thành công",
                Response = new AuthResponseDTO()
                {
                    AccessToken = token,
                    Roles = roles.ToList()
                }
            };
        }
    }
}