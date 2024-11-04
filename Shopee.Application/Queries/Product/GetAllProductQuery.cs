using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Models;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Product;

namespace Shopee.Application.Queries.Product
{
    public class GetAllProductQuery : IRequest<ApiReponse<Pagination<ProductResponseDTO>>>
    {
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, ApiReponse<Pagination<ProductResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiReponse<Pagination<ProductResponseDTO>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.ToPagination(
                request.PageIndex.GetValueOrDefault(1), // Nếu PageIndex là null, sử dụng giá trị mặc định là 1
                request.PageSize.GetValueOrDefault(20),
                null, // No filter
                query => query.Include(p => p.Cateogry), // Include related Category entity
                p => p.Name, // Order by Name property
                true // Ascending order
            );
            var result = _mapper.Map<Pagination<ProductResponseDTO>>(products);
            return new ApiReponse<Pagination<ProductResponseDTO>>()
            {
                Message = "Lấy sản phẩm thành công",
                Response = result
            };
        }
    }
}