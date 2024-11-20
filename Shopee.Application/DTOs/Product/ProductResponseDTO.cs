using Newtonsoft.Json;
using Shopee.Domain.Entities;
using System.Text.Json.Serialization;

namespace Shopee.Application.DTOs.Product
{
    public class ProductResponseDTO
    {
        [JsonProperty("productId")]
        public Guid Id { get; set; }
        public decimal? Price { get; set; }
        public float? Rating { get; set; }
        public int? Stock { get; set; }
        public int? Sold { get; set; }
        public int? View { get; set; }
        public string? Name { get; set; } = null!;
        public string? Image { get; set; }
        public IList<string>? Images { get; set; }
        public string? CateogryId { get; set; }
    }
}