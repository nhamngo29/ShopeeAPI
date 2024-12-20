﻿using MediatR;
using Shopee.Application.DTOs;

namespace Shopee.Application.Queries.User
{
    public class GetAllUsersDetailsQuery : IRequest<List<UserResponseDTO>>
    {
        //public string UserId { get; set; }
    }

    public class GetAllUsersDetailsQueryHandler : IRequestHandler<GetAllUsersDetailsQuery, List<UserResponseDTO>>
    {
        private readonly IIdentityService _identityService;

        public GetAllUsersDetailsQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<List<UserResponseDTO>> Handle(GetAllUsersDetailsQuery request, CancellationToken cancellationToken)
        {
            var users = await _identityService.GetAllUsersAsync();
            var userDetails = users.Select(x => new UserResponseDTO()
            {
                Id = x.id,
                Email = x.email,
                UserName = x.userName
                //Roles = (IList<string>)_identityService.GetUserRolesAsync(x.id) // Converstion problem
            }).ToList();

            foreach (var user in userDetails)
            {
                user.Roles = await _identityService.GetUserRolesAsync(user.Id);
            }
            return userDetails;
        }
    }
}