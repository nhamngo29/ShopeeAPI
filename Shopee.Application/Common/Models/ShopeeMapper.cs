using AutoMapper;
using Shopee.Application.DTOs.Product;
using Shopee.Domain.Entities;

namespace Shopee.Application.Common.Models;

public class ShopeeMapper : Profile
{
    public ShopeeMapper()
    {
        CreateMap<Product, ProductResponseDTO>().ReverseMap();
    }
}