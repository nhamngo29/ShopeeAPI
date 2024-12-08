using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Order;
public class CreateOrderDto
{
    public Guid? UserId { get; set; } // Id của người dùng (nếu người dùng đã đăng nhập)
    public string? SessionId { get; set; } // Id của phiên làm việc (dành cho khách vãng lai)
    public decimal TotalAmount { get; set; } // Tổng giá trị đơn hàng
    public string? Address { get; set; } // Địa chỉ giao hàng
    public string? Note { get; set; } // Ghi chú từ khách hàng (nếu có)
    public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>(); // Danh sách sản phẩm
}