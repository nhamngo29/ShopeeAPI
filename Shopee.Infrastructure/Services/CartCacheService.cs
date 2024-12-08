using Newtonsoft.Json;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.DTOs.Cart;
using Shopee.Application.Services.Interfaces;
using Shopee.Domain.Constants;

namespace Shopee.Infrastructure.Services;
public class CartCacheService : ICartCacheService
{
    private readonly ICacheService _cache; // Redis cache

    public CartCacheService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<Guid> DeleteCartItemAsync(string sessionId, string productId)
    {
        var cacheKey = $"{Constants.CART_CACHE_KEY_PREFIX}{sessionId}";
        var cartItemsJson = await _cache.GetCacheReponseAync(cacheKey);

        var cartItems = string.IsNullOrEmpty(cartItemsJson)
            ? new List<AddToCartReponseDTO>()
            : JsonConvert.DeserializeObject<IList<AddToCartReponseDTO>>(cartItemsJson);

        var cartItem = cartItems.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem != null)
        {
            cartItems.Remove(cartItem);
        }
        else
        {
            throw new NotFoundException("Sản phẩm không tồn tại trong giỏ hàng");
        }

        await _cache.SetCacheReponseAync(cacheKey, cartItems, TimeSpan.FromHours(24));
        return cartItem.CartItemId;
    }
}