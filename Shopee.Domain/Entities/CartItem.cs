using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        [ForeignKey("Cart")]
        public Guid CartId { get; set; }
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public Cart Cart { get; set; } // Điều hướng đến Cart

        public Product Product { get; set; } // Điều hướng đến Product
    }
}
