using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Entities;
public class Cart : BaseEntity
{
    [Key]
    public Guid UserId { get; set; }
    public float Discount { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}
