using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Cart
{
    public class GetCartItemReponseDTO
    {
        public int UniqueTotalCartItem { get; set; }
        public int TotalCartItem { get; set; }
        public IList<CartItemProductDTO> Products { get; set; }
        public static GetCartItemReponseDTO Default => new GetCartItemReponseDTO
        {
            TotalCartItem = 0,
            UniqueTotalCartItem = 0,
            Products = new List<CartItemProductDTO>()
        };
    }
}
