using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopee.Application.Commands.User.Create;
using Shopee.Application.Commands.User.Delete;
using Shopee.Application.Commands.User.Update;
using Shopee.Application.DTOs;
using Shopee.Application.Queries.User;

namespace Shopee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator) : ControllerBase
    {

        [HttpPost("Create")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> CreateUser(CreateUserCommand command)
        {
            return Ok(await mediator.Send(command));
        }

        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            return Ok(await mediator.Send(new Application.Queries.User.GetUserQuery()));
        }

        [HttpDelete("Delete/{userId}")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await mediator.Send(new DeleteUserCommand() { Id = userId });
            return Ok(result);
        }

        [HttpGet("GetUserDetails/{userId}")]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            var result = await mediator.Send(new GetUserDetailsQuery() { UserId = userId });
            return Ok(result);
        }

        [HttpGet("GetUserDetailsByUserName/{userName}")]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<IActionResult> GetUserDetailsByUserName(string userName)
        {
            var result = await mediator.Send(new GetUserDetailsByUserNameQuery() { UserName = userName });
            return Ok(result);
        }

        [HttpPost("AssignRoles")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> AssignRoles(AssignUsersRoleCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("EditUserRoles")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> EditUserRoles(UpdateUserRolesCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("GetAllUserDetails")]
        [ProducesDefaultResponseType(typeof(UserDetailsResponseDTO))]
        public async Task<IActionResult> GetAllUserDetails()
        {
            
            var result = await mediator.Send(new GetAllUsersDetailsQuery());
            return Ok(result);
        }

        [HttpPut("EditUserProfile/{id}")]
        [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> EditUserProfile(string id, [FromBody] EditUserProfileCommand command)
        {
            if (id == command.Id)
            {
                var result = await mediator.Send(command);
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}