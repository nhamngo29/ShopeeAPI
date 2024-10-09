using Shopee.Application.Commands.Auth;
using Shopee.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("Login")]
        [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> Login([FromBody] AuthCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            return Ok();
        }
        //[HttpPost("Signin")]
        //[ProducesDefaultResponseType(typeof(int))]
        //public async Task<IActionResult> Signin([FromBody] SigninCommand command)
        //{
        //    return Ok(await _mediator.Send(command));
        //}
    }
}
