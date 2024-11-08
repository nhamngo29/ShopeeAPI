using AutoMapper;
using MediatR;
using Shopee.Application.Common.Models;
using Shopee.Application.DTOs;
using Shopee.Application.DTOs.Category;
using Shopee.Application.DTOs.Product;
using Shopee.Application.Queries.Product;
using Shopee.Domain.Entities;

namespace Shopee.Application.Queries.Category;
public class GetAllCategoryQuery : IRequest<ApiReponse<IEnumerable<CategoryResponseDTO>>>
{

}

public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, ApiReponse<IEnumerable<CategoryResponseDTO>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiReponse<IEnumerable<CategoryResponseDTO>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.GetAll();
        var result = _mapper.Map<IEnumerable<CategoryResponseDTO>>(categories);
        return new ApiReponse<IEnumerable<CategoryResponseDTO>>()
        {
            Message = "Danh sách category",
            Response = result
        };
    }
}