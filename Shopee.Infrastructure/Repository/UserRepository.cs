using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;

namespace Shopee.Infrastructure.Repository;

public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(DbContext dbContext) : base(dbContext)
    {
        _db = (_db ?? (ApplicationDbContext)dbContext);
    }
}
