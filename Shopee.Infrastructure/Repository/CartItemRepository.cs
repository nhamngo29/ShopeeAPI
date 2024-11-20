using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Interfaces.Repository;
using Shopee.Domain.Entities;
using Shopee.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Infrastructure.Repository
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        private readonly ApplicationDbContext _db;

        public CartItemRepository(DbContext dbContext) : base(dbContext)
        {
            _db = (_db ?? (ApplicationDbContext)dbContext);
        }
    }
}
