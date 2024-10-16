using Shopee.Application.Common.Exceptions;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs;
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
        private readonly ITokenService _tokenGenerator;
        private readonly IIdentityService _identityService;

        public AuthCommandHandler(IIdentityService identityService, ITokenService tokenGenerator)
        {
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<ApiReponse<AuthResponseDTO>> Handle(AuthCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.SigninUserAsync(request.UserName, request.Password);

            if (!result)
            {
                return new ApiReponse<AuthResponseDTO>()
                {
                    Message = "Đăng nhập không thành công",
                    StatusCode = 400,
                    IsSuccess = false,
                };
            }

            var (userId, fullName, userName, email, roles) = await _identityService.GetUserDetailsAsync(await _identityService.GetUserIdAsync(request.UserName));

            (string token, DateTime expiration) = _tokenGenerator.GenerateJWTToken((userId, userName, roles, email, fullName));
            string refreshToken = _tokenGenerator.GenerateRefreshToken();
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
