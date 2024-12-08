using Shopee.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Services.Interfaces;
public interface IOrderService
{
    Task<Guid> CreateOrderAsync(CreateOrderDto orderDto, CancellationToken cancellationToken);
    Task UpdateOrderStatusAsync(Guid orderId, string newStatus, CancellationToken cancellationToken);
    Task<OrderDto> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken);
}