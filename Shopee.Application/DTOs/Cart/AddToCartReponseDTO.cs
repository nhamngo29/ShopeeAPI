using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Cart
{
    public class AddToCartReponseDTO
    {
        public Guid CartItemId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        [JsonIgnore]
        public DateTime AddedToCartAt { get; set; }=DateTime.Now;
    }
}
