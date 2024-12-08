using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;

namespace Shopee.Infrastructure.Repository;
public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
{
    private readonly ApplicationDbContext _db;
    public OrderItemRepository(DbContext context) : base(context)
    {
        _db = (_db ?? (ApplicationDbContext)context);
    }
}
