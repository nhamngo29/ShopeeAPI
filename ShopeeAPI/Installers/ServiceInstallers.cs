using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shopee.Application.Common;
using Shopee.Application.Common.Exceptions;
using System.Text;

namespace Shopee.API.Installers
{
    public class ServiceInstallers : IInstaller
    {
        public void InstrallServices(IServiceCollection services, IConfiguration configuration)
        {
            var settingConfiguration = configuration.Get<SettingConfiguration>()
                                       ?? throw ProgramException.AppsettingNotSetException();
            services.AddSingleton(settingConfiguration);
            services.AddWebAPIService(settingConfiguration);
            services.AddAppDI(configuration);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                        {
                            context.Token = context.Request.Cookies["X-Access-Token"];
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = settingConfiguration.Jwt.Audience,
                    ValidIssuer = settingConfiguration.Jwt.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settingConfiguration.Jwt.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            });
        }
    }
}