using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopee.API.Attibutes;
using Shopee.Application.Commands.User.Create;
using Shopee.Application.Commands.User.Delete;
using Shopee.Application.Commands.User.Update;
using Shopee.Application.DTOs;
using Shopee.Application.Queries.Product;
using Shopee.Application.Queries.User;

namespace Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var query = new GetProfileUserQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}