using Shopee.API.Extensions;
using Shopee.Application.Common;
using System.Reflection;

namespace Shopee.API;

public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIService(this IServiceCollection services, SettingConfiguration appSettings)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddSwaggerOpenAPI(appSettings);
        //services.SetupHealthCheck(appSettings);
        return services;
    }

}
