using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopee.Application.Queries.Product;

namespace Shopee.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IMediator mediator) : ControllerBase
    {
        [HttpGet( "products")]
        public async Task<IActionResult> GetAllProduct([FromQuery] GetAllProductQuery command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
