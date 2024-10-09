
using Shopee.Application.Common.Interfaces;
using Shopee.Domain.Configuration;
using Shopee.Infrastructure.Services;
using StackExchange.Redis;

namespace Shopee.API.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstrallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);
            services.AddSingleton(redisConfiguration);
            ConfigurationOptions option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = redisConfiguration.ConnectTimeOut,
                DefaultDatabase = 0,
                EndPoints = { redisConfiguration.ConnectionString },
                Password = redisConfiguration.Password
            };
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(option));
            services.AddStackExchangeRedisCache(option => { option.Configuration = redisConfiguration.ConnectionString; });
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}
