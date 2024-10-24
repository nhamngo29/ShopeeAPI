using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopee.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; private set; }

        public BaseEntity()
        {
            ModifiedDate = DateTime.Now;
        }
    }
}