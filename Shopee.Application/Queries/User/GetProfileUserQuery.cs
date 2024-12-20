﻿using AutoMapper;
using MediatR;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs;
namespace Shopee.Application.Queries.User
{
    public class GetProfileUserQuery:IRequest<ApiReponse<UserResponseDTO>>
    {

    }
    public class GetProfileUserQueryHandler : IRequestHandler<GetProfileUserQuery, ApiReponse<UserResponseDTO>>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        public GetProfileUserQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        public async Task<ApiReponse<UserResponseDTO>> Handle(GetProfileUserQuery request, CancellationToken cancellationToken)
        {

            var userId = _currentUserService.GetUserId();//lấy user id từ session
            var user = await _identityService.GetUserById(userId);
            if (user == null)//Kiểm tra refresh token có hợp lệ hay không
                throw UserException.UserUnauthorizedException();
            var userReponse = _mapper.Map<UserResponseDTO>(user);
            return new ApiReponse<UserResponseDTO>()
            {
                Message="Lấy dữ liệu thành công",
                Response= userReponse
            };
        }
    }
}
