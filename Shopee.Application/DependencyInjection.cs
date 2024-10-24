using Microsoft.Extensions.DependencyInjection;
using Shopee.Application.Services;
using System.Reflection;

namespace Shopee.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddScoped<IMailService, MailService>();
            return services;
        }
    }
}