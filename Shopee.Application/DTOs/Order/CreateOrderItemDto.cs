using FluentValidation;
namespace Shopee.Application.DTOs.Order;
public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        // Validate ProductId
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId không được để trống.");
        // Validate Quantity
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng sản phẩm phải lớn hơn 0.");
    }
}
public class CreateOrderItemDto
{
    public Guid ProductId { get; set; } // Id của sản phẩm
    public int Quantity { get; set; } // Số lượng sản phẩm
}
