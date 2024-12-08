using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;

namespace Shopee.Application.Commands.User.Update;
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(t => t.FullName)
            .NotEmpty()
            .WithMessage("Vui lòng nhập họ và tên.");

        RuleFor(t => t.Address)
            .NotEmpty()
            .WithMessage("Vui lòng nhập địa chỉ.");

        RuleFor(t => t.DateOfBirth)
            .NotEmpty()
            .WithMessage("Vui lòng nhập ngày sinh.")
            .LessThan(DateTime.Now)
            .WithMessage("Ngày sinh phải nhỏ hơn ngày hiện tại.");

        RuleFor(t => t.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Vui lòng nhập số điện thoại.")
            .Matches(@"^\d+$")
            .WithMessage("Số điện thoại chỉ được chứa chữ số.")
            .Matches(@"^0[3|5|7|8|9]\d{8}$")
            .WithMessage("Số điện thoại không đúng định dạng tại Việt Nam.");

        RuleFor(t => t.ProfilePricture)
            .Custom((file, context) =>
            {
                if (file != null)
                {
                    const int maxFileSizeInBytes = 500 * 1024; // 500KB
                    if (file.Length > maxFileSizeInBytes)
                    {
                        context.AddFailure("ProfilePricture", "Kích thước ảnh không được vượt quá 500KB.");
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        context.AddFailure("ProfilePricture", "Định dạng ảnh không hợp lệ. Chỉ hỗ trợ .jpg, .jpeg, .png.");
                    }
                }
            });
    }
}
public class UpdateProfileCommand : IRequest<ApiReponse<UserResponseDTO>>
{
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public IFormFile? ProfilePricture { get; set; }
}
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiReponse<UserResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    public UpdateProfileCommandHandler(ICurrentUser currentUser, IIdentityService identityService, IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _currentUser = currentUser;
        _mapper = mapper;
        _fileService = fileService;
    }
    public async Task<ApiReponse<UserResponseDTO>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetCurrentUserId();//lấy user id từ session
        var user = await _identityService.GetUserById(userId.ToString());
        if (user == null)
            throw UserException.UserUnauthorizedException();
        long maxSizeInBytes = 1 * 1024 * 1024; // 1 MB
        if (request.ProfilePricture != null && request.ProfilePricture.Length > maxSizeInBytes)
        {
            throw new BadRequestException("File size exceeds the maximum limit of 1 MB.");
        }
        if (request.ProfilePricture != null)
        {
            string createdImageName = await _fileService.SaveFileAsync(request.ProfilePricture, Constants.ALLOWED_FILE_EXTENSIONS);
            if (!string.IsNullOrEmpty(createdImageName)) user.Avatar = createdImageName;
        }
        if (!string.IsNullOrEmpty(request.FullName)) user.FullName = request.FullName;
        if (request.DateOfBirth.HasValue) user.BirthOfDay = request.DateOfBirth.Value;
        if (!string.IsNullOrEmpty(request.PhoneNumber)) user.PhoneNumber = request.PhoneNumber;
        _unitOfWork.User.Update(user);
        var reuslt = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (reuslt < 1)
            throw new BadRequestException("Cập nhật không thành công vui lòng thử lại");
        var userReponse = _mapper.Map<UserResponseDTO>(user);
        return new ApiReponse<UserResponseDTO>()
        {
            Message = "Cập nhật thông tin người dùng thành công",
            Response = userReponse
        };
    }
}
