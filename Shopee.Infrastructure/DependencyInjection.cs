using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shopee.Application.Common.Interfaces;
using Shopee.Domain.Entities;
using Shopee.Domain.Repositories.Command.Base;
using Shopee.Domain.Repositories.Query.Base;
using Shopee.Infrastructure.Data;
using Shopee.Infrastructure.Repository.Command.Base;
using Shopee.Infrastructure.Repository.Query.Base;
using Shopee.Infrastructure.Services;

namespace Shopee.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DatabaseShopeeClone")
                               ?? configuration.GetConnectionString("DatabaseShopeeClone");

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
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
            services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
            return services;
        }
    }
}
