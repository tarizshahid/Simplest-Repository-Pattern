using System.Linq.Expressions;

namespace BlazorGlobeApp.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<T> GetAsync(short id);
        Task<IQueryable<T>> GetAllAsync(bool trackChanges);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IQueryable<TResult>> GetSelectedColumnsAsync<TResult>(Expression<Func<T, TResult>> selector, bool trackChanges);
        Task<IQueryable<TResult>> GetSelectedColumnsAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filter, bool trackChanges);
        Task<IQueryable<TResult>> GetSelectedColumnsWithPaginationAsync<TResult>(Expression<Func<T, TResult>> selector, bool trackChanges, int pageNumber, int pageSize);
        Task<IQueryable<TResult>> GetSelectedColumnsWithPaginationAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filter, bool trackChanges, int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<IQueryable<TResult>> GetSelectedColumnsWithPaginationAndSearchAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> searchFilter, int pageNumber, int pageSize, bool trackChanges);
    }
}
