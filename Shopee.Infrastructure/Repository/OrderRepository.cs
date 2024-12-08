using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;

namespace Shopee.Infrastructure.Repository;
internal class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _db;
    public OrderRepository(DbContext context) : base(context)
    {
        _db = (_db ?? (ApplicationDbContext)context);
    }
}
