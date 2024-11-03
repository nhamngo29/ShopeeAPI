using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Interfaces.Repository;
using Shopee.Domain.Entities;
using Shopee.Infrastructure.Data;

namespace Shopee.Infrastructure.Repository;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(DbContext dbContext) : base(dbContext)
    {
        _db = (_db ?? (ApplicationDbContext)dbContext);
    }
}