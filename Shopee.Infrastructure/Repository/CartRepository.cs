using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Interfaces.Repository;
using Shopee.Domain.Entities;
using Shopee.Infrastructure.Data;
namespace Shopee.Infrastructure.Repository
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _db;

        public CartRepository(DbContext dbContext) : base(dbContext)
        {
            _db = (_db ?? (ApplicationDbContext)dbContext);
        }
    }
}
