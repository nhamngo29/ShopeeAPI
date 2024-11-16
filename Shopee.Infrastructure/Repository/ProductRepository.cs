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
    }
}