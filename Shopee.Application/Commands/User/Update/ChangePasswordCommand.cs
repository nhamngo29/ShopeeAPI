using AutoMapper;
using FluentValidation;
using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;

namespace Shopee.Application.Commands.User.Update;
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        // Password cũ là bắt buộc
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu cũ phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu cũ phải chứa ít nhất một chữ cái viết hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu cũ phải chứa ít nhất một chữ cái viết thường.")
            .Matches(@"\d").WithMessage("Mật khẩu cũ phải chứa ít nhất một chữ số.")
            .Matches(@"[#?!@$%^&*-]").WithMessage("Mật khẩu cũ phải chứa ít nhất một ký tự đặc biệt.");

        // Mật khẩu mới là bắt buộc
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự.")
            .Matches(@"[A-Z]").WithMessage("Mật khẩu mới phải chứa ít nhất một chữ cái viết hoa.")
            .Matches(@"[a-z]").WithMessage("Mật khẩu mới phải chứa ít nhất một chữ cái viết thường.")
            .Matches(@"\d").WithMessage("Mật khẩu mới phải chứa ít nhất một chữ số.")
            .Matches(@"[#?!@$%^&*-]").WithMessage("Mật khẩu mới phải chứa ít nhất một ký tự đặc biệt.")
            .NotEqual(x => x.Password).WithMessage("Mật khẩu mới không được giống mật khẩu cũ.");

        // Xác nhận mật khẩu phải khớp với mật khẩu mới
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
            .Equal(x => x.NewPassword).WithMessage("Xác nhận mật khẩu không khớp với mật khẩu mới.");
    }
}
public class ChangePasswordCommand : IRequest<ApiReponse<UserResponseDTO>>
{
    public string Password { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiReponse<UserResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    public ChangePasswordCommandHandler(ICurrentUser currentUser, IIdentityService identityService, IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _currentUser = currentUser;
        _mapper = mapper;
        _fileService = fileService;
    }

    public async Task<ApiReponse<UserResponseDTO>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetCurrentUserId();//lấy user id từ session
        var user = await _identityService.GetUserById(userId.ToString());
        var resusltChange =await _identityService.ChangePassword(user, request.Password, request.NewPassword);
        if(!resusltChange)
            throw new ConflictException(
                  new Dictionary<string, string[]>
                  {
                    { "password", new[] { "Mật khẩu không chính xác.!" } }
                  }
              );
        var result=_mapper.Map<UserResponseDTO>(user);
        return new ApiReponse<UserResponseDTO>()
        {
            Message = "Thay đổi mật khẩu thành công.!",
            Response = result
        };
    }
}