using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Domain.Entities;

public class ImageProduct : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Url { get; set; } = null!;
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; } = null!;
}