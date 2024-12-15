using FluentValidation;
using MediatR;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs;
using Shopee.Application.Services.Interfaces;

namespace Shopee.Application.Commands.Auth
{
    public class SignUpRequestValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpRequestValidator()
        {
            RuleFor(t => t.FullName).NotEmpty().WithMessage("Vui lòng nhập họ và tên.");
            RuleFor(t => t.Email).NotEmpty().WithMessage("Vui lòng nhập Email.").EmailAddress().WithMessage("Vui lòng nhập đúng định dạng Email.");
            RuleFor(t => t.Password)
    .NotEmpty().WithMessage("Vui lòng nhập mật khẩu.")
    .MinimumLength(8).WithMessage("Mật khẩu phải dài hơn 8 ký tự.")
    .Matches("[A-Z]").WithMessage("Mật khẩu phải có một ký tự hoa.")
    .Matches("[a-z]").WithMessage("Mật khẩu phải có một ký tự thường.")
    .Matches("[0-9]").WithMessage("Mật khẩu phải có một ký tự số.")
    .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải có một ký tự đặt biệt.");
            RuleFor(t => t.PasswordConfirm)
    .NotEmpty().WithMessage("Vui lòng nhập lại mật khẩu.")
    .Equal(t => t.Password).WithMessage("Nhập lại mật khẩu không chính xác.");
        }
    }

    public class SignUpCommand : IRequest<ApiReponse<AuthResponseDTO>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class SignUpCommandHandler(IIdentityService identityService, ITokenService tokenService) : IRequestHandler<SignUpCommand, ApiReponse<AuthResponseDTO>>
    {
        public async Task<ApiReponse<AuthResponseDTO>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            if (!await identityService.IsUniqueUserName(request.UserName))
                throw new Common.Exceptions.ConflictException(
                 new Dictionary<string, string[]>
                 {
                    { "userName", new[] { "Tên đăng nhập đã tồn tại" } }
                 }
             );
            var result = await identityService.CreateUserAsync(request.UserName, request.Password, request.Email, request.FullName, ["User"]);
            var resultSignUp = await identityService.SigninUserAsync(request.UserName, request.Password);
            if (!resultSignUp)
            {
                return new ApiReponse<AuthResponseDTO>()
                {
                    Message = "Đăng ký thành công vui lòng đăng nhập",
                };
            }

            string token =await tokenService.GenerateToken(result);
            string refreshToken = tokenService.GenerateRefreshToken();
            result.RefreshToken = refreshToken;
            await identityService.SaveRefreshTokenUser(result);//save refresh token
            return new ApiReponse<AuthResponseDTO>()
            {
                Message = "Đăng ký thành công",
                Response = new AuthResponseDTO()
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                }
            };
        }
    }
}