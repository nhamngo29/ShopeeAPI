using AutoMapper;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Cart;
using Shopee.Application.DTOs.Category;
using Shopee.Application.DTOs.Order;
using Shopee.Application.DTOs.Product;
using Shopee.Domain.Common;
using Shopee.Domain.Entities;

namespace Shopee.Application.Common.Models;

public class ShopeeMapper : Profile
{
    public ShopeeMapper()
    {
        CreateMap<Pagination<Product>, Pagination<ProductResponseDTO>>().ReverseMap();
        CreateMap<Product, ProductResponseDTO>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id)) // Ánh xạ Id -> ProductId
            .ForMember(dest => dest.CateogryId, opt => opt.MapFrom(src => src.IdCateogry.ToString()))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.Url).ToList())).ReverseMap();
        CreateMap<Category, CategoryResponseDTO>().ReverseMap();
        CreateMap<Product, CartItemProductDTO>()
                        .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id)) // Ánh xạ Id -> ProductId
    .ReverseMap(); // Để ánh xạ ngược nếu cần
        CreateMap<ApplicationUser, UserResponseDTO>().ReverseMap();
        CreateMap<Order, OrderDto>()
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
           .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));


        // Map từ CreateOrderDto sang entity Order
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()); // Tính toán ở tầng nghiệp vụ
    }
}