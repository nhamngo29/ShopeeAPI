using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Entities;
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; } // Foreign Key từ bảng Order
    public Guid ProductId { get; set; } // Foreign Key từ bảng Product
    public decimal Price { get; set; } // Giá sản phẩm tại thời điểm mua
    public int Quantity { get; set; } // Số lượng mua

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!; // Navigation Property tới Order
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; } // Navigation Property tới Product
}
