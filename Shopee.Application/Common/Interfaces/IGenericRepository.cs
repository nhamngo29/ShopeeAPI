using Shopee.Application.Common.Models;
using System.Linq.Expressions;

namespace Shopee.Application.Common.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

    Task<bool> AnyAsync();

    Task<int> CountAsync(Expression<Func<T, bool>> filter);

    Task<int> CountAsync();

    Task<T> GetByIdAsync(object id);

    Task<Pagination<T>> ToPagination(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Expression<Func<T, object>>? orderBy = null,
        string? ascending = null);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IQueryable<T>>? include = null);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    Task Delete(object id);

    Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IQueryable<T>>? include = null);
}