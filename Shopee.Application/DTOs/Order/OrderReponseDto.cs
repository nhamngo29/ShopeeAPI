using Shopee.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Order;
public class OrderReponseDto
{
    public string Status { get; set; } = OrderStatus.Pending.ToString(); // Trạng thái mặc định
    public decimal TotalAmount { get; set; }
    public IList<OrderItemReponseDto>? Items { get; set; }=new List<OrderItemReponseDto>();
}
