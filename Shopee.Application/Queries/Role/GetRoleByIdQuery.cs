using MediatR;
using Shopee.Application.DTOs;

namespace Shopee.Application.Queries.Role
{
    public class GetRoleByIdQuery : IRequest<RoleResponseDTO>
    {
        public string RoleId { get; set; }
    }

    public class GetRoleQueryByIdHandler : IRequestHandler<GetRoleByIdQuery, RoleResponseDTO>
    {
        private readonly IIdentityService _identityService;

        public GetRoleQueryByIdHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<RoleResponseDTO> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _identityService.GetRoleByIdAsync(request.RoleId);
            return new RoleResponseDTO() { Id = role.id, RoleName = role.roleName };
        }
    }
}