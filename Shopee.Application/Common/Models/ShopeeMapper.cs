using AutoMapper;
using Shopee.Application.DTOs.Category;
using Shopee.Application.DTOs.Product;
using Shopee.Domain.Entities;

namespace Shopee.Application.Common.Models;

public class ShopeeMapper : Profile
{
    public ShopeeMapper()
    {
        CreateMap<Pagination<Product>, Pagination<ProductResponseDTO>>().ReverseMap();
        CreateMap<Product, ProductResponseDTO>().ReverseMap();
        CreateMap<Category, CategoryResponseDTO>().ReverseMap();
    }
}