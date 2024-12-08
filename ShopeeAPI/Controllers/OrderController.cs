using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopee.Application.Commands.Order;
using Shopee.Application.DTOs.Order;
using Shopee.Application.Queries.Order;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Shopee.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Order([FromBody] List<CreateOrderItemDto> items)
    {
        var command = new CreateOrderCommand
        {
            Items = items
        };
        var result=await mediator.Send(command);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> Order([FromQuery] GetOrderQuery request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}
