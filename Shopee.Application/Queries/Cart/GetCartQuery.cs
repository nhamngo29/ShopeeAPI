using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Cart;
using Shopee.Domain.Entities;

namespace Shopee.Application.Queries.Cart
{
    public class GetCartQuery : IRequest<ApiReponse<GetCartItemReponseDTO>>
    {

    }
    public class GetCartQueryHandler: IRequestHandler<GetCartQuery, ApiReponse<GetCartItemReponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache; // Redis cache
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCartQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUser currentUser, ICacheService cache, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiReponse<GetCartItemReponseDTO>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                var sessionId = Utility.GetSessionId(_httpContextAccessor);
                return new ApiReponse<GetCartItemReponseDTO>()
                {
                    Message = "Lấy sản phẩm trong giỏ hàng thành công",
                    Response = await GetCartItemInCacheAsync(sessionId)
                };
            }
            else
            {
                return new ApiReponse<GetCartItemReponseDTO>()
                {
                    Message = "Lấy sản phẩm trong giỏ hàng thành công",
                    Response = await GetCartItemInDbAsync(userId)
                };

            }    

            return null;
        }
        public async Task<GetCartItemReponseDTO?> GetCartItemInDbAsync(Guid userId)
        {
            var cart = await _unitOfWork.Cart.FirstOrDefaultAsync(t => t.UserId == userId);
            if(cart is null)
                return GetCartItemReponseDTO.Default;
            var cartItems = await _unitOfWork.CartItem.GetFilter(t => t.CartId == cart.Id);
            var productIds = cartItems.Select(x => x.ProductId.ToString()).Distinct().ToHashSet();
            if (!productIds.Any())
                return GetCartItemReponseDTO.Default;
            var products = await _unitOfWork.Products.GetProductsByIdsAsync(productIds.ToList());
            var mappedCartItems = _mapper.Map<IList<CartItemProductDTO>>(products);

            // Gán Quantity từ cartItems vào các sản phẩm đã ánh xạ
            foreach (var cartItem in cartItems)
            {
                var product = mappedCartItems.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                if (product != null)
                {
                    product.Quantity = cartItem.Quantity;  // Gán Quantity từ cartItem vào product
                    product.CartItemId = cartItem.Id.ToString();
                    product.AddedToCartAt = cartItem.CreateAt;
                }
            }
            var totalQuantity = cartItems.Sum(x => x.Quantity);
            return new GetCartItemReponseDTO()
            {
                TotalCartItem = cartItems.Count,
                UniqueTotalCartItem = totalQuantity,
                Products = mappedCartItems.OrderByDescending(t => t.AddedToCartAt).ToList()
            };
        }
        public async Task<GetCartItemReponseDTO?> GetCartItemInCacheAsync(string sessionId)
        {
            var cacheKey = $"{Constants.CART_CACHE_KEY_PREFIX}{sessionId}";
            var cartItemsJson = await _cache.GetCacheReponseAync(cacheKey);
            if (string.IsNullOrEmpty(cartItemsJson))
                return GetCartItemReponseDTO.Default;
            IList<CartItemProductDTO> cartItems;
            try
            {
                cartItems = JsonConvert.DeserializeObject<IList<CartItemProductDTO>>(cartItemsJson) ?? new List<CartItemProductDTO>();
            }
            catch (JsonException)
            {
                return GetCartItemReponseDTO.Default;
            }
            var productIds = cartItems.Select(x => x.ProductId.ToString()).Distinct().ToHashSet();

            if (!productIds.Any())
                return GetCartItemReponseDTO.Default;
            var products = await _unitOfWork.Products.GetProductsByIdsAsync(productIds.ToList());
            var mappedCartItems = _mapper.Map<IList<CartItemProductDTO>>(products);

            // Gán Quantity từ cartItems vào các sản phẩm đã ánh xạ
            foreach (var cartItem in cartItems)
            {
                var product = mappedCartItems.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                if (product != null)
                {
                    product.Quantity = cartItem.Quantity;  // Gán Quantity từ cartItem vào product
                    product.CartItemId = cartItem.CartItemId;
                    product.AddedToCartAt = cartItem.AddedToCartAt;
                }
            }
            var totalQuantity = cartItems.Sum(x => x.Quantity);
            return new GetCartItemReponseDTO()
            {
                TotalCartItem = cartItems.Count,
                UniqueTotalCartItem = totalQuantity,
                Products = mappedCartItems.OrderByDescending(t => t.AddedToCartAt).ToList()
            };
        }
    }
}
