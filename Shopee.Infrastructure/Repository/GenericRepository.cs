using Microsoft.EntityFrameworkCore;
using Shopee.Domain.Common;
using Shopee.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Shopee.Infrastructure.Repository;

public class GenericRepository<T>(DbContext context) : IGenericRepository<T> where T : class
{
    protected DbSet<T> _dbSet = context.Set<T>();

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<T> entities)
        => await _dbSet.AddRangeAsync(entities);

    #region Read

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        => await _dbSet.AnyAsync(filter);

    public async Task<bool> AnyAsync()
        => await _dbSet.AnyAsync();

    public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
    {
        return filter == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(filter);
    }

    public async Task<int> CountAsync()
        => await _dbSet.CountAsync();

    public async Task<T?> GetByIdAsync(object id)
        => await _dbSet.FindAsync(id);

    public async Task<Pagination<T>> ToPagination(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Expression<Func<T, object>>? orderBy = null,
        string? ascending=null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        orderBy ??= x => EF.Property<object>(x, "Id");

        query = ascending=="asc" ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var result = await Pagination<T>.ToPagedList(query, pageIndex, pageSize);

        return result;
    }

    public async Task<T?> FirstOrDefaultAsync(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet.IgnoreQueryFilters().AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(filter);
    }

    #endregion Read

    #region Update & delete

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void UpdateRange(IEnumerable<T> entities)
        => _dbSet.UpdateRange(entities);

    public void Delete(T entity)
        => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities)
        => _dbSet.RemoveRange(entities);

    public async Task Delete(object id)
    {
        T entity = await GetByIdAsync(id);
        Delete(entity);
    }

    public async Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet;
        if (include != null)
        {
            query = include(query);
        }
        return await query.ToListAsync();
    }

    public async Task<IList<TResult>> GetFilter<TResult>(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IQueryable<T>>? include = null,
    Expression<Func<T, TResult>>? selector = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Nếu selector là null, mặc định select toàn bộ thực thể
        if (selector == null)
        {
            return await query.Cast<TResult>().ToListAsync(); // Dùng Cast cho trường hợp T và TResult giống nhau
        }

        return await query.Select(selector).ToListAsync();
    }
    public async Task<IList<T>> GetFilter(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet.IgnoreQueryFilters().AsNoTracking();
        if (include != null)
        {
            query = include(query);
        }
        query = query.Where(filter);
        return await query.ToListAsync();
    }
    #endregion Update & delete
}