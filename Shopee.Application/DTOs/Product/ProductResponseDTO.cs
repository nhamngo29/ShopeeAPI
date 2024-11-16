using Shopee.Domain.Entities;
using System.Text.Json.Serialization;

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
        public IList<string>? Images { get; set; }
        public string? CateogryId { get; set; }
    }
}