using AutoMapper;
using Shopee.Application.Common.Exceptions;
using Shopee.Application.DTOs.Order;
using Shopee.Application.Services.Interfaces;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.UnitOfWork;

namespace Shopee.Application.Services;
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderDto orderDto, CancellationToken cancellationToken)
    {
        // Kiểm tra các điều kiện nghiệp vụ trước khi tạo đơn hàng
        if (orderDto == null || !orderDto.Items.Any())
        {
            throw new ArgumentException("Danh sách sản phẩm không được để trống.");
        }

        // Ánh xạ từ CreateOrderDto sang entity Order
        var order = _mapper.Map<Order>(orderDto);

        // Cập nhật Status mặc định
        order.Status = "Pending";

        // Tính toán lại TotalAmount và kiểm tra tồn kho
        decimal totalAmount = 0;
        foreach (var item in orderDto.Items)
        {
            // Kiểm tra tồn kho (ví dụ: kiểm tra từng sản phẩm)
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product == null || product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Sản phẩm {item.ProductId} không đủ số lượng.");
            }

            // Cập nhật tồn kho
            product.Stock -= item.Quantity;

            // Tính toán tổng giá trị đơn hàng
            totalAmount += product.Price * item.Quantity;

            // Thêm sản phẩm vào đơn hàng
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            };
            order.AddItem(orderItem); // Sử dụng phương thức AddItem
        }

        // Lưu đơn hàng vào database
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }


    public async Task UpdateOrderStatusAsync(Guid orderId, string newStatus, CancellationToken cancellationToken)
    {
        // Lấy đơn hàng
        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            throw new NotFoundException("Đơn hàng không tồn tại.");
        }

        // Cập nhật trạng thái
        order.Status = newStatus;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderDto> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            throw new NotFoundException("Đơn hàng không tồn tại.");
        }
        return _mapper.Map<OrderDto>(order);
    }

    public async Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            throw new NotFoundException("Đơn hàng không tồn tại.");
        }

        _unitOfWork.Orders.Delete(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
