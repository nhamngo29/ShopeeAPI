using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Common.Exceptions
{
    public class HttpStatusException : Exception
    {
        public int StatusCode { get; }

        public HttpStatusException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
