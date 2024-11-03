using System.ComponentModel.DataAnnotations;

namespace Shopee.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}