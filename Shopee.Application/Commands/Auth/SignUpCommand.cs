using FluentValidation;
using MediatR;
using Shopee.Application.DTOs;

namespace Shopee.Application.Commands.Auth
{
    public class SignUpRequestValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpRequestValidator()
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
            var validator = new SignUpRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new Common.Exceptions.ValidationException(validationResult.Errors);
            }
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

            var (userId, fullName, userName, email, roles) = await identityService.GetUserDetailsAsync(await identityService.GetUserIdAsync(request.UserName));

            string token = tokenService.GenerateJWTToken((userId, userName, roles, email, fullName));


            return new ApiReponse<AuthResponseDTO>()
            {
                Message = "Đăng ký thành công",
                Response = new AuthResponseDTO()
                {
                    Roles = roles.ToList(),
                    AccessToken = token
                }
            };
        }
    }
}