using Shopee.Application.DTOs.Product;

namespace Shopee.Application.DTOs.Cart
{
    public class CartItemProductDTO: ProductResponseDTO
    {
        public int Quantity { get; set; }

    }
}
