using Shopee.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs.Order;
public class OrderItemReponseDto:ProductResponseDTO
{
    public int Quantity { get; set; }
    public string? OrderItemId { get;set; }
    public decimal PriceBuy { get; set; }
}
