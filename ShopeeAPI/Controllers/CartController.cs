using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopee.Application.Commands.CartItem;
using Shopee.Application.Queries.Cart;
using Shopee.Application.Queries.Category;

namespace Shopee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(IMediator mediator) : ControllerBase
    {
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> CartToCart(AddCartItemCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("get-items-in-cart")]
        public async Task<IActionResult> GetItemsInCart()
        {
            var result = await mediator.Send(new GetCartQuery());
            return Ok(result);
        }
        [HttpDelete("delete-cart-item/{productId}")]
        public async Task<IActionResult> RemoveCartItems([FromQuery] DeleteCartItemCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("update-cart-item")]
        public async Task<IActionResult> UpdateCartImtes(UpdateCartItemCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
