using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.DTOs
{
    public class ApiReponse<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; } = null!;
        public int StatusCode { get; set; }
        public T? Response { get; set; }
    }
}
