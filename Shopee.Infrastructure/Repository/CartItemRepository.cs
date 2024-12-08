using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;

namespace Shopee.Infrastructure.Repository;

public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
    private readonly ApplicationDbContext _db;

    public CartItemRepository(DbContext dbContext) : base(dbContext)
    {
        _db = (_db ?? (ApplicationDbContext)dbContext);
    }
}
