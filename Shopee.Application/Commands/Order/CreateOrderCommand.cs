using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Order;
using Shopee.Application.Services.Interfaces;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.UnitOfWork;
using System.Text.Json.Serialization;
namespace Shopee.Application.Commands.Order;
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotNull().WithMessage("Danh sách sản phẩm không được để trống.")
            .NotEmpty().WithMessage("Danh sách sản phẩm phải có ít nhất một sản phẩm.");

        // Validate từng Item trong danh sách
        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemDtoValidator());
    }
}

public class CreateOrderCommand : IRequest<ApiReponse<string>>
{
    public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>(); // Danh sách sản phẩm
    [JsonIgnore]
    public string SessionId { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
}
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiReponse<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly ICartCacheService  _CartCacheService;

    public CreateOrderCommandHandler(ICurrentUser currentUser, IIdentityService identityService, IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IHttpContextAccessor httpContextAccessor,ICartCacheService cartCacheService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _currentUser = currentUser;
        _mapper = mapper;
        _fileService = fileService;
        _httpContextAccessor = httpContextAccessor;
        _CartCacheService = cartCacheService;
    }

    public async Task<ApiReponse<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetCurrentUserId(); // Lấy userId từ session
        Guid resultAdd = Guid.Empty;
        if (userId == Guid.Empty)
        {
            // Nếu người dùng không đăng nhập, sử dụng sessionId
            var sessionId = Utility.GetSessionId(_httpContextAccessor);
            request.SessionId = sessionId;
            // Thực hiện các thao tác cần thiết với sessionId nếu cần
        }
        else
        {
            // Người dùng đã đăng nhập, xử lý đơn hàng và lưu vào cơ sở dữ liệu
            request.UserId = userId.ToString();
        }
        resultAdd = await AddItemOrder(request, cancellationToken);
        // Trả về phản hồi API
        return new ApiReponse<string>
        {
            Response = resultAdd.ToString(),
            Message = resultAdd == Guid.Empty ? "Tạo đơn hàng thất bại" : "Tạo đơn hàng thành công"
        };
    }

    private async Task<Guid> AddItemOrder(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Tạo entity Order từ DTO
            var order = new Domain.Entities.Order
            {
                UserId = request.UserId,
                Status = "Pending", // Trạng thái đơn hàng là "Pending" khi mới tạo
                SessionId=request.SessionId,
            };

            var cacheTasks = new List<Task>(); // Danh sách các task để xử lý cache

            foreach (var item in request.Items)
            {
                // Kiểm tra tồn tại của sản phẩm
                var product = await _unitOfWork.Products.FirstOrDefaultAsync(t => t.Id == item.ProductId);
                if (product == null)
                {
                    throw new BadRequestException($"Sản phẩm với ID {item.ProductId} không tồn tại.");
                }

                // Kiểm tra tồn kho
                if (product.Stock < item.Quantity)
                {
                    throw new BadRequestException($"Sản phẩm {item.ProductId} không đủ số lượng trong kho.");
                }

                // Cập nhật tồn kho của sản phẩm
                product.Stock -= item.Quantity;

                // Thêm sản phẩm vào đơn hàng
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
                order.AddItem(orderItem); // Sử dụng phương thức AddItem trong Order để thêm sản phẩm vào đơn hàng

                // Thêm tác vụ xóa giỏ hàng vào danh sách nếu có SessionId
                if (!string.IsNullOrEmpty(order.SessionId))
                {
                    cacheTasks.Add(_CartCacheService.DeleteCartItemAsync(order.SessionId, item.ProductId.ToString()));
                }
            }

            // Lưu đơn hàng vào cơ sở dữ liệu
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await Task.WhenAll(cacheTasks);
            return order.Id;
        }
        catch (Exception ex)
        {

            throw ;
        }
    }
}