using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Domain.Entities;

public class Product : BaseEntity
{
    public Guid IdCateogry { get; set; }
    public decimal Price { get; set; }
    public float? Rating { get; set; }
    public int Stock { get; set; }
    public int? Sold { get; set; }
    public int? View { get; set; }
    public string Name { get; set; } = null!;
    public virtual List<ImageProduct>? Images { get; set; }
    [ForeignKey("IdCateogry")]
    public virtual Category Cateogry { get; set; }
    public string? Image { get; set; }

}