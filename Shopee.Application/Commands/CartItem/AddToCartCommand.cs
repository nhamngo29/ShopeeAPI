using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Cart;
using Shopee.Domain.Entities;

namespace Shopee.Application.Commands.CartItem
{
    public class AddToCartCommand : IRequest<ApiReponse<AddToCartReponseDTO>>
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ApiReponse<AddToCartReponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ICacheService _cache; // Redis cache
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddToCartCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ICurrentUser currentUser,
            ICacheService cache,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiReponse<AddToCartReponseDTO>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.GetCurrentUserId();
            AddToCartReponseDTO result=null;
            if (userId == Guid.Empty)
            {
                // Người dùng chưa đăng nhập: Lưu vào Redis cache
                var sessionId = Utility.GetSessionId(_httpContextAccessor);
                result= await AddOrUpdateCartItemInCacheAsync(sessionId, request);
            }
            else
            {
                // Người dùng đã đăng nhập: Lưu vào cơ sở dữ liệu
                await _unitOfWork.ExecuteTransactionAsync(async () =>
                {
                    var cart = await GetOrCreateCartAsync(userId);
                    result= await AddOrUpdateCartItemAsync(cart.Id, request.ProductId, request.Quantity);
                }, cancellationToken);
            }

            return new ApiReponse<AddToCartReponseDTO>
            {
                Message = "Thêm sản phẩm vào giỏ hàng thành công",
                Response = result
            };

        }

        private async Task<AddToCartReponseDTO> AddOrUpdateCartItemInCacheAsync(string sessionId, AddToCartCommand command)
        {
            var cacheKey = $"{Constants.CART_CACHE_KEY_PREFIX}{sessionId}";
            var cartItemsJson = await _cache.GetCacheReponseAync(cacheKey);

            // Danh sách giỏ hàng hiện tại
            var cartItems = string.IsNullOrEmpty(cartItemsJson)
                ? new List<AddToCartReponseDTO>() // Nếu không có, tạo mới danh sách
                : JsonConvert.DeserializeObject<IList<AddToCartReponseDTO>>(cartItemsJson);
            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var cartItem = cartItems.FirstOrDefault(x => x.ProductId == command.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity += command.Quantity;
            }
            else
            {
                cartItem = new AddToCartReponseDTO()
                {
                    ProductId = command.ProductId,
                    Quantity = command.Quantity,
                    CartItemId = Guid.NewGuid().ToString()
                };
                cartItems.Add(cartItem);
            }
            await _cache.SetCacheReponseAync(cacheKey, cartItems, TimeSpan.FromHours(24));
            return cartItem;
        }


        private async Task<Domain.Entities.Cart> GetOrCreateCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Cart.FirstOrDefaultAsync(t => t.UserId == userId);
            if (cart == null)
            {
                cart = new Domain.Entities.Cart { UserId = userId };
                await _unitOfWork.Cart.AddAsync(cart);
            }
            return cart;
        }

        private async Task<AddToCartReponseDTO> AddOrUpdateCartItemAsync(Guid cartId, string productId, int quantity)
        {
            if (!Guid.TryParse(productId, out Guid pdid))
            {
                throw new BadRequestException("Mã sản phẩm không hợp lệ.");
            }
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == pdid);
            if (product == null)
            {
                throw new NotFoundException($"Sản phẩm không tồn tại, vui lòng thử lại.");
            }
            var cartItem = await _unitOfWork.CartItem.FirstOrDefaultAsync(t => t.CartId == cartId && t.ProductId == pdid);
            if (cartItem == null)
            {
                cartItem = new Domain.Entities.CartItem
                {
                    CartId = cartId,
                    ProductId = pdid,
                    Quantity = quantity
                };
                await _unitOfWork.CartItem.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _unitOfWork.CartItem.Update(cartItem);
            }

            return new AddToCartReponseDTO()
            {
                CartItemId = cartItem.Id.ToString(),
                Price = product.Price, //lấy giá hiện tại
                ProductId = productId,
                Quantity = cartItem.Quantity
            };
        }
    }
}
