using AutoMapper;
using MediatR;
using Shopee.Application.DTOs.Product;

namespace Shopee.Application.Queries.Product
{
    public class GetAllProductQuery : IRequest<List<ProductResponseDTO>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, List<ProductResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ProductResponseDTO>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.ToPagination(request.PageIndex, request.PageSize);

            return _mapper.Map<List<ProductResponseDTO>>(products);
        }
    }
}