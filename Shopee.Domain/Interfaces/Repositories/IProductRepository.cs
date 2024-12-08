namespace Shopee.Domain.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IList<Product>> GetProductsByIdsAsync(IList<string> productIds);
}