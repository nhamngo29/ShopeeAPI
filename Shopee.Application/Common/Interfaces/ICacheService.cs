namespace Shopee.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task SetCacheReponseAync(string cacheKy, object reponse, TimeSpan timeOut);

        Task<string> GetCacheReponseAync(string cacheKey);

        public Task RemoveCacheAsyncReponse(string pattern);
    }
}