using Microsoft.OpenApi.Models;
using Shopee.Application.Common;

namespace Shopee.API.Extensions;

public static class SwaggerExtension
{
    private static readonly string[] Value = ["Bearer"];

    public static IServiceCollection AddSwaggerOpenAPI(this IServiceCollection services, SettingConfiguration appSettings)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("OpenAPISpecification",
            new OpenApiInfo
            {
                Title = appSettings.ApplicationDetail.ApplicationName,
                Version = "v1",
                Description = appSettings.ApplicationDetail.Description,
                Contact = new OpenApiContact
                {
                    Email = "nhamngoinfo@gmail.com",
                    Name = "Nham Ngo",
                    Url = new Uri(appSettings.ApplicationDetail.ContactWebsite),
                }
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement { { securityScheme, Value } };

            options.AddSecurityRequirement(securityRequirement);
        });
        return services;
    }
}
