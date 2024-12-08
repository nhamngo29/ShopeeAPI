using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Product;
using Shopee.Domain.Common;

namespace Shopee.Application.Queries.Product
{
    public class GetAllProductQuery : IRequest<ApiReponse<Pagination<ProductResponseDTO>>>
    {
        public int? Page { get; set; } = 1;
        public string? OrderBy { get; set; }
        public string? Order { get; set; } = null;
        public string? CategoryId { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public int? RatingFilter { get; set; }
        public string? Keyword { get; set; }
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
                Constants.PAGE_SIZE_PRODUCT, //page size
                t =>
                    (string.IsNullOrEmpty(request.CategoryId) || t.IdCateogry.ToString() == request.CategoryId) &&
                    (!request.MinPrice.HasValue || t.Price >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue || t.Price <= request.MaxPrice.Value) &&
                    (!request.RatingFilter.HasValue || t.Rating >= request.RatingFilter.Value) &&
                    (string.IsNullOrEmpty(request.Keyword) || t.Name.Contains(request.Keyword)), // Tìm kiếm LIKE theo từ khóa,
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