using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopee.API.Attibutes;
using Shopee.Application.Queries.Product;

namespace Shopee.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        [HttpGet( "products")]
        [CacheAttribute(10*60)]//10 phút
        public async Task<IActionResult> GetAllProduct([FromQuery] GetAllProductQuery command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("product/{id}")]
        [CacheAttribute(10 * 60)]//10 phút
        public async Task<IActionResult> GetProductDetail([FromRoute] Guid id)
        {
            var command = new GetProductDetailQuery { Id = id };
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
