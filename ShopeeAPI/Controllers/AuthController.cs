using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopee.API.Controllers;
using Shopee.Application.Commands.Auth;
using Shopee.Application.DTOs;

namespace Shopee.Api.Controllers;
public class AuthController(IMediator mediator) : BaseController
{

    [HttpPost("sign-in")]
    [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
    public async Task<IActionResult> Login([FromBody] SignInCommand command) => Ok(await mediator.Send(command));

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshCommand command) => Ok(await mediator.Send(command));

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        return Ok(await mediator.Send(command));
    }
    [HttpPost("sign-out")]
    [Authorize]
    public async Task<IActionResult> Logout(SignOutCommand command) => Ok(await mediator.Send(command));
}