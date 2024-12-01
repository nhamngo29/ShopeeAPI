using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.Common.Interfaces.Repository;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Commands.User.Update
{
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
            var reuslt=await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (reuslt < 1)
                throw new BadRequestException("Cập nhật không thành công vui lòng thử lại");
            var userReponse = _mapper.Map<UserResponseDTO>(user);
            return new ApiReponse<UserResponseDTO>()
            {
                Message="Cập nhật thông tin người dùng thành công",
                Response= userReponse
            };
        }
    }
}
