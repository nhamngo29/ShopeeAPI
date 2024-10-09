using MediatR;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Commands.Auth
{
    //public class SigninCommand : IRequest<ApiReponse<AuthResponseDTO>>
    //{
    //    public string UserName { get; set; }
    //    public string Password { get; set; }
    //}

    //partial class SignInCommand : IRequestHandler<SigninCommand, ApiReponse<AuthResponseDTO>>
    //{
    //    private readonly IIdentityService _identityService;
    //    public SignInCommand(IIdentityService identityService)
    //    {
    //        _identityService = identityService;
    //    }

    //    public async Task<ApiReponse<AuthResponseDTO>> Handle(SigninCommand request, CancellationToken cancellationToken)
    //    {
    //        var result = await _identityService.CreateUserAsync(request.UserName, request.Password, request.UserName, null, ["User"]);
    //        if (result.isSucceed) {
    //            return new ApiReponse<AuthResponseDTO>
    //            {
    //                IsSuccess = true,
    //                StatusCode = 200,
    //                Message = "Đăng nhập thành công",
    //                Response=
    //            };
    //        }
    //    }
    //}
}
