using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Interfaces.Repository;
using Shopee.Application.DTOs.Product;
using Shopee.Domain.Entities;
using Shopee.Infrastructure.Data;

namespace Shopee.Infrastructure.Repository
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(DbContext dbContext) : base(dbContext)
        {
            _db = (_db ?? (ApplicationDbContext)dbContext);
        }
        public async Task<IList<Product>> GetProductsByIdsAsync(IList<string> productIds)
        {
            var productIdSet = new HashSet<string>(productIds); // Chuyển sang HashSet để tăng tốc độ tra cứu

            return await _db.Products
                .Where(p => productIdSet.Contains(p.Id.ToString()))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}