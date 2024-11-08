using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopee.Application.Queries.Category;
using Shopee.Application.Queries.Product;

namespace Shopee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(IMediator mediator) : ControllerBase
    {
        [HttpGet("categoies")]
        public async Task<IActionResult> GetAllCategory([FromQuery] GetAllCategoryQuery command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
