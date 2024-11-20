using MediatR;
using Shopee.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Commands.Cart
{
    public class CreateCartCommand : IRequest<ApiReponse<string>>
    {
        public Guid UserId { get; set; }
        public float Discount { get; set; }
    }
}
