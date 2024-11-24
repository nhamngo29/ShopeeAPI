using AutoMapper;
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
    public class AddToCartCommand : IRequest<ApiReponse<CartItemProductDTO>>
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ApiReponse<CartItemProductDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ICacheService _cache; // Redis cache
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AddToCartCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ICurrentUser currentUser,
            ICacheService cache,
            IHttpContextAccessor httpContextAccessor,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ApiReponse<CartItemProductDTO>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.ProductId, out Guid pdid))
            {
                throw new BadRequestException("Mã sản phẩm không hợp lệ.");
            }
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == pdid);
            if (product is null)
            {
                throw new NotFoundException("Sản phẩm không tồn tại vui lòng thử lại");
            }
            var userId = _currentUser.GetCurrentUserId();
            Guid resultAdd = Guid.Empty;
            if (userId == Guid.Empty)
            {
                // Người dùng chưa đăng nhập: Lưu vào Redis cache
                var sessionId = Utility.GetSessionId(_httpContextAccessor);
                resultAdd = await AddOrUpdateCartItemInCacheAsync(sessionId, request);
            }
            else
            {
                // Người dùng đã đăng nhập: Lưu vào cơ sở dữ liệu
                await _unitOfWork.ExecuteTransactionAsync(async () =>
                {
                    var cart = await GetOrCreateCartAsync(userId);
                    resultAdd = await AddOrUpdateCartItemAsync(cart.Id, request.ProductId, request.Quantity);
                }, cancellationToken);
            }
            var result= _mapper.Map<CartItemProductDTO>(product);
            result.Quantity=request.Quantity;
            result.CartItemId = resultAdd.ToString();
            return new ApiReponse<CartItemProductDTO>
            {
                Message = "Thêm sản phẩm vào giỏ hàng thành công",
                Response = result
            };

        }

        private async Task<Guid> AddOrUpdateCartItemInCacheAsync(string sessionId, AddToCartCommand command)
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
                cartItem.AddedToCartAt = DateTime.Now;
            }
            else
            {
                cartItem = new AddToCartReponseDTO()
                {
                    ProductId = command.ProductId,
                    Quantity = command.Quantity,
                    CartItemId = Guid.NewGuid()
                };
                cartItems.Add(cartItem);
            }
            await _cache.SetCacheReponseAync(cacheKey, cartItems, TimeSpan.FromHours(24));
            return cartItem.CartItemId;
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

        private async Task<Guid> AddOrUpdateCartItemAsync(Guid cartId, string productId, int quantity)
        {
            if (!Guid.TryParse(productId, out Guid pdid))
            {
                throw new BadRequestException("Mã sản phẩm không hợp lệ.");
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

            return cartItem.Id;
        }
    }
}
