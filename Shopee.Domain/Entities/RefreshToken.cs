using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public string IdUser { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        [ForeignKey("Id")]
        public virtual ApplicationUser User { get; set; } = null!;

        public RefreshToken(string idUser, string token, DateTime expirationDate)
        {
            IdUser = idUser;
            Token = token;
            ExpirationDate = expirationDate;
        }
    }
}
