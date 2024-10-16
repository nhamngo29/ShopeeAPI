using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresRefreshToken { get; set; }
        public List<string> Roles { get; set; }

    }
}
