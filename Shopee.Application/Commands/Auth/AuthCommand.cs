using Shopee.Application.Common.Exceptions;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs;
using Shopee.Domain.Entities;
using MediatR;



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
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthCommandHandler(IIdentityService identityService, ITokenService tokenGenerator, IRefreshTokenService refreshTokenService)
        {
            _identityService = identityService;
            _tokenService = tokenGenerator;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ApiReponse<AuthResponseDTO>> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.SigninUserAsync(request.UserName, request.Password);

            if (!result)
            {
                throw new HttpStatusException(401, "Tên tài khoản của bạn hoặc Mật khẩu không đúng, vui lòng thử lại");
            }

            var (userId, fullName, userName, email, roles) = await _identityService.GetUserDetailsAsync(await _identityService.GetUserIdAsync(request.UserName));

            (string token, DateTime expiration) = _tokenService.GenerateJWTToken((userId, userName, roles, email, fullName));
            (string refreshToken, DateTime expirationRefreshToken) = _tokenService.GenerateRefreshToken();
            await _refreshTokenService.SaveRefreshToken(new RefreshToken(userId, refreshToken, expirationRefreshToken));
            return new ApiReponse<AuthResponseDTO>()
            {
                Message = "Đăng nhập thành công",
                StatusCode = 200,
                IsSuccess = true,
                Response = new AuthResponseDTO()
                {
                    AccessToken = token,
                    Expires = expiration,
                    RefreshToken = refreshToken,
                    Roles=roles.ToList()
                }
            };
        }
    }
}
