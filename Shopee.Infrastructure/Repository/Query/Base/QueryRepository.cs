using Microsoft.Extensions.Configuration;
using Shopee.Domain.Repositories.Query.Base;
using Shopee.Infrastructure.Data;

namespace Shopee.Infrastructure.Repository.Query.Base
{
    // Generic Query repository class
    public class QueryRepository<T> : DbConnector, IQueryRepository<T> where T : class
    {
        public QueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
