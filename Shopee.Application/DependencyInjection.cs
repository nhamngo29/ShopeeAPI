using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shopee.Application.Commands.Auth;
using Shopee.Application.Commands.Order;
using Shopee.Application.Commands.User.Update;
using Shopee.Application.Common.Behaviours;
using Shopee.Application.Services;
using System.Reflection;

namespace Shopee.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssemblyContaining<SignUpRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateProfileCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                ctg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                ctg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                ctg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            });
            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IMailService, MailService>();
            return services;
        }
    }
}