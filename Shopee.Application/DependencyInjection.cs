using Microsoft.Extensions.DependencyInjection;
using Shopee.Application.Common.Interfaces;
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
            services.AddSingleton<ICurrentUser, CurrentUser>();

            services.AddSingleton<ICookieService, CookieService>();
            services.AddScoped<IMailService, MailService>();
            return services;
        }
    }
}