using Shopee.Application.DTOs.Product;

namespace Shopee.Application.DTOs.Cart
{
    public class CartItemProductDTO: ProductResponseDTO
    {
        public int Quantity { get; set; }
        public string CartItemId { get; set; }
        public DateTime AddedToCartAt { get; set; }
    }
}