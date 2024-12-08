using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopee.Domain.Entities;

namespace Shopee.Infrastructure.Data.Configurations
{
    class OrderConffiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasMany(o => o.OrderItems)
        .WithOne(oi => oi.Order)
        .HasForeignKey(oi => oi.OrderId)
        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
