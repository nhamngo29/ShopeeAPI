using Shopee.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Application.DTOs.Product
{
    public class ProductResponseDTO
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public float? Rating { get; set; }
        public int Quantity { get; set; }
        public int? Sold { get; set; }
        public int? View { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public List<ImageProduct>? Images { get; set; }
        public Category? Cateogry { get; set; }
    }
}