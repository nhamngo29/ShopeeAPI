using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.Repositories;

namespace Shopee.Infrastructure.Repository;

public class ImageProductRepository : GenericRepository<ImageProduct>, IImageProductRepository
{
    private readonly ApplicationDbContext _db;

    public ImageProductRepository(DbContext dbContext) : base(dbContext)
    {
        _db = (_db ?? (ApplicationDbContext)dbContext);
    }
}