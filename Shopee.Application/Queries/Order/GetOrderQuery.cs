using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopee.Application.Commands.Order;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Order;
using Shopee.Application.DTOs.Product;
using Shopee.Application.Services.Interfaces;
using Shopee.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Queries.Order;
public class GetOrderQuery : IRequest<ApiReponse<IEnumerable<OrderReponseDto>>>
{
   public OrderStatus status { get; set; }
}
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, ApiReponse<IEnumerable<OrderReponseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly ICartCacheService _CartCacheService;
    public GetOrderQueryHandler(ICurrentUser currentUser, IIdentityService identityService, IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IHttpContextAccessor httpContextAccessor, ICartCacheService cartCacheService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _currentUser = currentUser;
        _mapper = mapper;
        _fileService = fileService;
        _httpContextAccessor = httpContextAccessor;
        _CartCacheService = cartCacheService;
    }
    public async Task<ApiReponse<IEnumerable<OrderReponseDto>>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
       
        var userId = _currentUser.GetCurrentUserId();
        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException();
        var orders = await _unitOfWork.Orders.GetFilter(t => t.UserId == userId.ToString() && t.Status==request.status.ToString(), query => query.Include(o => o.OrderItems));
        var listOrder = new List<OrderReponseDto>();
        foreach (var item in orders)
        {
            var orderItems = await _unitOfWork.OrderItems.GetFilter(t => item.Id==t.OrderId, query => query.Include(o => o.Product), t => new OrderItemReponseDto { CateogryId = t.Product.IdCateogry.ToString(), Image = t.Product.Image, Name = t.Product.Name, Price = t.Product.Price, Quantity = t.Quantity, PriceBuy = t.Price,ProductId=t.ProductId });
            var order = new OrderReponseDto()
            {
                Status = item.Status,
                TotalAmount = item.TotalAmount,
                Items = orderItems
            };
            listOrder.Add(order);
        }
        return new ApiReponse<IEnumerable<OrderReponseDto>>()
        {
            Message="Lấy danh sách đơn hàng thành công",
            Response= listOrder
        };
    }
}