﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Product;

namespace Shopee.Application.Queries.Product
{
    public class GetProductDetailQuery : IRequest<ApiReponse<ProductResponseDTO>>
    {
        public Guid Id { get; set; }
    }

    public class GetProductDetailQueryHandler : IRequestHandler<GetProductDetailQuery, ApiReponse<ProductResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductDetailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiReponse<ProductResponseDTO>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(t=>t.Id== request.Id,t=>t.Include(t=>t.Images));
            var result = _mapper.Map<ProductResponseDTO>(product);
            return new ApiReponse<ProductResponseDTO>()
            {
                Message = "Chi tiết sản phẩm",
                Response = result
            };
        }
    }
}
