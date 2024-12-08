using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Services.Interfaces;
public interface ICartCacheService
{
    Task<Guid> DeleteCartItemAsync(string sessionId, string productId);
    // Các phương thức liên quan đến cache giỏ hàng khác (nếu có)
}