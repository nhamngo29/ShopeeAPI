using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopee.Domain.Entities;

namespace Shopee.Infrastructure.Data.Configurations
{
    public class ImageProductConfiguration : IEntityTypeConfiguration<ImageProduct>
    {
        public void Configure(EntityTypeBuilder<ImageProduct> builder)
        {
            builder.ToTable("ImageProducts");
            builder.HasKey(x => x.Id);
        }
    }
}
