using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shopee.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonIgnore]
        public DateTime CreateAt { get;private set; } = DateTime.Now;
        [JsonIgnore]
        public DateTime UpdateAt { get; private set; }=DateTime.Now;
        [JsonIgnore]
        public Guid? CreatedBy { get; set; }
        [JsonIgnore]
        public Guid? UpdateBy { get; set; }
    }
}