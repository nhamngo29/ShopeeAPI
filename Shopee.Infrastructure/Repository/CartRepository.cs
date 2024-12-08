using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;
namespace Shopee.Infrastructure.Repository;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    private readonly ApplicationDbContext _db;

    public CartRepository(DbContext dbContext) : base(dbContext)
    {
        _db = (_db ?? (ApplicationDbContext)dbContext);
    }
}
