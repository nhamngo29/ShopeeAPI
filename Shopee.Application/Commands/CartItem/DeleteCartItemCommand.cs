using MediatR;
using Shopee.Application.DTOs.Cart;
using Shopee.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Exceptions;
using Newtonsoft.Json;
using Shopee.Domain.Entities;

namespace Shopee.Application.Commands.CartItem
{
    public class DeleteCartItemCommand : IRequest<ApiReponse<string>>
    {
        public string[] ProductsId { get; set; }
    }
    public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, ApiReponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ICacheService _cache; // Redis cache
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public DeleteCartItemCommandHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, ICacheService cache, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ApiReponse<string>> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            foreach (var productId in request.ProductsId)
            {
                try
                {
                    if (!Guid.TryParse(productId, out Guid pdid))
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
                        var result = await DeleteCartItemInCacheAsync(sessionId, productId);
                    }
                    else//Xử lý trong database khi người dùng đăng nhập
                    {

                    }
                }
                catch (Exception)
                {
                    throw new NotImplementedException();
                }
            }
            return new ApiReponse<string>
            {
                Message = $"Xóa sản phẩm khỏi giỏ hàng thành công"
            };
        }
        private async Task<Guid> DeleteCartItemInCacheAsync(string sessionId, string productId)
        {
            var cacheKey = $"{Constants.CART_CACHE_KEY_PREFIX}{sessionId}";
            var cartItemsJson = await _cache.GetCacheReponseAync(cacheKey);
            // Danh sách giỏ hàng hiện tại
            var cartItems = string.IsNullOrEmpty(cartItemsJson)
                ? new List<AddToCartReponseDTO>() // Nếu không có, tạo mới danh sách
                : JsonConvert.DeserializeObject<IList<AddToCartReponseDTO>>(cartItemsJson);
            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
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
}
