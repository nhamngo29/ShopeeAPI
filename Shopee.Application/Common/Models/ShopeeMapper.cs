using AutoMapper;
using Shopee.Application.DTOs.Cart;
using Shopee.Application.DTOs.Category;
using Shopee.Application.DTOs.Product;
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
    }
}