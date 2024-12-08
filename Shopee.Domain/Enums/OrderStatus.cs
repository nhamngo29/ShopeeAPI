using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Enums;
public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Canceled
}