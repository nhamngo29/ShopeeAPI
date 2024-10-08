using Shopee.Infrastructure;
using Shopee.Application;
namespace Shopee.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI().AddInfrastructureDI(configuration);

            return services;
        }
    }
}
