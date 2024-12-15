using Shopee.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Entities;
public class Order : BaseEntity
{
    public string? UserId { get; set; }
    public string Status { get; set; } = OrderStatus.Pending.ToString(); // Trạng thái mặc định
    public string? SessionId { get; set; }
    public decimal TotalAmount { get; private set; } // Chỉ có thể được thay đổi thông qua logic nghiệp vụ

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    // Navigation Property
    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    public Order()
    {
    }

    public Order(string? userId, string? sessionId)
    {
        UserId = userId;
        SessionId = sessionId;
    }

    public void AddItem(OrderItem newItem)
    {
        if (newItem == null)
        {
            throw new ArgumentNullException(nameof(newItem), "Sản phẩm không được để trống.");
        }

        if (newItem.Quantity <= 0)
        {
            throw new ArgumentException("Số lượng sản phẩm phải lớn hơn 0.", nameof(newItem.Quantity));
        }

        if (newItem.Price < 0)
        {
            throw new ArgumentException("Giá sản phẩm không được nhỏ hơn 0.", nameof(newItem.Price));
        }

        var existingItem = OrderItems.FirstOrDefault(item => item.ProductId == newItem.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += newItem.Quantity;
        }
        else
        {
            OrderItems.Add(newItem);
        }

        RecalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        var item = OrderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            OrderItems.Remove(item);
            RecalculateTotal();
        }
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus.ToString();
    }

    private void RecalculateTotal()
    {
        TotalAmount = OrderItems.Sum(x => x.Price * x.Quantity);
    }
}
