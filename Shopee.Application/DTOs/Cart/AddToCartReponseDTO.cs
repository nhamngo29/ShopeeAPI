using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Cart
{
    public class AddToCartReponseDTO
    {
       public string CartItemId { get; set; }
       public string ProductId { get; set; }
       public int Quantity { get; set; }
       public decimal? Price { get; set; }
    }
}
