using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Models;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Product;
using Shopee.Application.Utilities;

namespace Shopee.Application.Queries.Product
{
    public class GetAllProductQuery : IRequest<ApiReponse<Pagination<ProductResponseDTO>>>
    {
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? OrderBy { get; set; }
        public string? Order { get; set; } = null;
        public string? CategoryId { get; set; }
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
            var orderByExpression = Utility.GetPropertyByString<Domain.Entities.Product>(request.OrderBy);
            var products = await _unitOfWork.Products.ToPagination(
                request.Page.GetValueOrDefault(1), // Nếu PageIndex là null, sử dụng giá trị mặc định là 1
                request.PageSize.GetValueOrDefault(20),
                string.IsNullOrEmpty(request.CategoryId)
                    ? null
                    : t => t.IdCateogry.ToString() == request.CategoryId, // Lọc theo CategoryId
                query => query.Include(p => p.Cateogry), // Include related Category entity
                orderByExpression, // Order by Name property
                request.Order // Ascending order
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