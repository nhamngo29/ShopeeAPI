using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Order;
public class OrderDto
{
    public Guid Id { get; set; } // Id của đơn hàng
    public Guid? UserId { get; set; } // Id của người dùng (nếu có)
    public string? UserName { get; set; } // Tên người dùng (nếu có)
    public string? SessionId { get; set; } // Id của phiên làm việc (dành cho khách vãng lai)
    public decimal TotalAmount { get; set; } // Tổng giá trị đơn hàng
    public string Status { get; set; } = string.Empty; // Trạng thái của đơn hàng
    public string? Address { get; set; } // Địa chỉ giao hàng
    public int ItemCount { get; set; }
    public string? Note { get; set; } // Ghi chú từ khách hàng
    public DateTime CreatedAt { get; set; } // Thời gian tạo đơn hàng
    public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật gần nhất (nếu có)
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>(); // Danh sách sản phẩm trong đơn hàng
}
