using Shopee.Domain.Entities;

namespace Shopee.Application.Common.Interfaces.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IList<Product>> GetProductsByIdsAsync(IList<string> productIds);
    }
}