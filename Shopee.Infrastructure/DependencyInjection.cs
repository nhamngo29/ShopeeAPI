using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopee.Application.Common.Interfaces;
using Shopee.Application.Services.Interfaces;
using Shopee.Domain.Entities;
using Shopee.Domain.Interfaces.UnitOfWork;
using Shopee.Infrastructure.Services;

namespace Shopee.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                               ?? configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(option =>
                option.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );
            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;   // Không yêu cầu số
                options.Password.RequireLowercase = false;   // Không yêu cầu chữ thường
                options.Password.RequireUppercase = false;   // Không yêu cầu chữ hoa
                options.Password.RequireNonAlphanumeric = false;   // Không yêu cầu ký tự đặc biệt
                options.Password.RequiredLength = 6;
            });
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICartCacheService, CartCacheService>();
            return services;
        }
    }
}