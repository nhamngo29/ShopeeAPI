using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;

namespace Shopee.Infrastructure.Repository;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(DbContext dbContext) : base(dbContext)
    {
        _db = (_db ?? (ApplicationDbContext)dbContext);
    }
}