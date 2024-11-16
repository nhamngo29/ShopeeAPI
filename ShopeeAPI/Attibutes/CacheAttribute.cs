using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shopee.Application.Common;
using Shopee.Application.Common.Interfaces;
using System.Text;

namespace Shopee.API.Attibutes
{
    public class CacheAttribute: Attribute,IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds=1000)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<SettingConfiguration>().RedisConfiguration;
            if (!cacheConfiguration.Enabled)
            {
                await next();
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cacheResponse = await cacheService.GetCacheReponseAync(cacheKey);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contextResult = new ContentResult
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contextResult;
                return;
            }
            var excutedContext = await next();
            if (excutedContext.Result is OkObjectResult objectResult)
            {
                await cacheService.SetCacheReponseAync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }
        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(t => t.Key))
            {
                keyBuilder.Append($"--{key}--{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
