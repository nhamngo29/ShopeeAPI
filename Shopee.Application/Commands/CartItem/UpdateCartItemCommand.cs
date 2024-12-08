using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Cart;


namespace Shopee.Application.Commands.CartItem
{
    public class UpdateCartItemCommand : IRequest<ApiReponse<string>>
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, ApiReponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ICacheService _cache; // Redis cache
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, ICacheService cache, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ApiReponse<string>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
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
                var result = await DeleteCartItemInCacheAsync(sessionId, request);
                if (result != Guid.Empty)
                {
                    return new ApiReponse<string>
                    {
                        Message = $"Cập nhật sản phẩm {product.Name} khỏi giỏ hàng thành công",
                        Response = result.ToString()
                    };
                }
            }
            else
            {

            }
            throw new NotImplementedException();
        }
        private async Task<Guid> DeleteCartItemInCacheAsync(string sessionId, UpdateCartItemCommand command)
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
                cartItem.Quantity = command.Quantity;
            }
            else
            {
                throw new NotFoundException("Sản phẩm không tồn tại trong giỏ hàng");
            }
            await _cache.SetCacheReponseAync(cacheKey, cartItems, TimeSpan.FromHours(24));
            return cartItem.CartItemId;
        }
    }
}
