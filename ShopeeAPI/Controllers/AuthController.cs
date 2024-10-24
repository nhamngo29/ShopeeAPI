using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shopee.API.Controllers;
using Shopee.Application.Commands.Auth;
using Shopee.Application.DTOs;

namespace Shopee.Api.Controllers;
public class AuthController : BaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("sign-in")]
    [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
    public async Task<IActionResult> Login([FromBody] AuthCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        return Ok();
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SigninCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return Ok();
    }
}