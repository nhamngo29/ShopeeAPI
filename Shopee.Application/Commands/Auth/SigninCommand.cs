using FluentValidation;
using MediatR;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs;

namespace Shopee.Application.Commands.Auth
{
    public class SigninRequestValidator : AbstractValidator<SigninCommand>
    {
        public SigninRequestValidator()
        {
            RuleFor(t => t.FullName).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(t => t.Email).NotEmpty().WithMessage("{PropertyName} is required").EmailAddress().WithMessage("Please enter email connert");
            RuleFor(t => t.Password)
    .NotEmpty().WithMessage("{PropertyName} is required.")
    .MinimumLength(8).WithMessage("{PropertyName} must be at least 8 characters long.")
    .Matches("[A-Z]").WithMessage("{PropertyName} must contain at least one uppercase letter.")
    .Matches("[a-z]").WithMessage("{PropertyName} must contain at least one lowercase letter.")
    .Matches("[0-9]").WithMessage("{PropertyName} must contain at least one number.")
    .Matches("[^a-zA-Z0-9]").WithMessage("{PropertyName} must contain at least one special character.");
            RuleFor(t => t.PasswordConfirm)
    .NotEmpty().WithMessage("{PropertyName} is required.")
    .Equal(t => t.Password).WithMessage("Password confirmation does not match the password.");
        }
    }
    public class SigninCommand : IRequest<ApiReponse<AuthResponseDTO>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    partial class SignInCommand : IRequestHandler<SigninCommand, ApiReponse<AuthResponseDTO>>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        public SignInCommand(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<ApiReponse<AuthResponseDTO>> Handle(SigninCommand request, CancellationToken cancellationToken)
        {
            var validator = new SigninRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new Common.Exceptions.ValidationException(validationResult.Errors);
            }
            if (!await _identityService.IsUniqueUserName(request.UserName))
                return new ApiReponse<AuthResponseDTO>()
                {
                    IsSuccess = false,
                    StatusCode = 422,
                    Message = $"{request.UserName} đã tồn tại"
                };
            var result = await _identityService.CreateUserAsync(request.UserName, request.Password, request.Email, request.FullName, ["User"]);
            var resultSignUp = await _identityService.SigninUserAsync(request.UserName, request.Password);
            if (!resultSignUp)
            {
                return new ApiReponse<AuthResponseDTO>()
                {
                    Message = "Đăng ký thành công vui lòng đăng nhập",
                    StatusCode = 200,
                    IsSuccess = true,
                };
            }

            var (userId, fullName, userName, email, roles) = await _identityService.GetUserDetailsAsync(await _identityService.GetUserIdAsync(request.UserName));

            (string token, DateTime expiration) = _tokenService.GenerateJWTToken((userId, userName, roles, email, fullName));
            string refreshToken = _tokenService.GenerateRefreshToken();
            return new ApiReponse<AuthResponseDTO>()
            {
                Message = "Đăng ký thành công",
                StatusCode = 200,
                IsSuccess = true,
                Response = new AuthResponseDTO()
                {
                    AccessToken = token,
                    Expires = expiration,
                    RefreshToken = refreshToken,
                    Roles = roles.ToList()
                }
            };

        }
    }
}
