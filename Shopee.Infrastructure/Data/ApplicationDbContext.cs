using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopee.Application.Common.Interfaces;
using Shopee.Domain.Entities;
using System.Reflection;
using System.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    public virtual DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        // Cấu hình khóa chính cho bảng Cart
        builder.Entity<Cart>()
            .HasKey(c => c.Id);

        // Cấu hình khóa chính cho bảng CartItem
        builder.Entity<CartItem>()
            .HasKey(ci => ci.Id);

        // Cấu hình mối quan hệ giữa CartItem và Cart (CartId là khóa ngoại)
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)  // Một CartItem chỉ thuộc về một Cart
            .WithMany(c => c.CartItems)  // Một Cart có thể chứa nhiều CartItem
            .HasForeignKey(ci => ci.CartId)  // CartId là khóa ngoại
            .OnDelete(DeleteBehavior.Cascade);  // Xóa Cart sẽ xóa các CartItem liên quan

        // Cấu hình mối quan hệ giữa CartItem và Product (ProductId là khóa ngoại)
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Product)  // Một CartItem chỉ liên kết với một Product
            .WithMany()  // Một Product có thể có nhiều CartItem
            .HasForeignKey(ci => ci.ProductId)  // ProductId là khóa ngoại
            .OnDelete(DeleteBehavior.Cascade);  // Xóa Product sẽ xóa các CartItem liên quan
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        SeedRoles(builder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
            new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
            );
    }
}