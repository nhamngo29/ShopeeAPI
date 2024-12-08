using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Order;
public class OrderItemDto
{
    public Guid ProductId { get; set; } // Id của sản phẩm
    public string ProductName { get; set; } = string.Empty; // Tên sản phẩm
    public int Quantity { get; set; } // Số lượng sản phẩm
    public decimal UnitPrice { get; set; } // Giá đơn vị tại thời điểm đặt hàng
    public decimal TotalPrice => UnitPrice * Quantity; // Tổng giá trị của sản phẩm trong đơn hàng
}
