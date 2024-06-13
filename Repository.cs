using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlazorGlobeApp.Data.Repositories

{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        /// <summary>
        /// Adds an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public async Task<IQueryable<T>> GetAllAsync(bool trackChanges) =>
        !trackChanges ? await Task.Run(() => _context.Set<T>().AsNoTracking()) : await Task.Run(() => _context.Set<T>());

        /// <summary>
        /// Gets an entity by id(int)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        /// <summary>
        /// Gets an entity by id(short)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetAsync(short id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets selected columns from a table based upon what selector is passed
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public async Task<IQueryable<TResult>> GetSelectedColumnsAsync<TResult>(Expression<Func<T, TResult>> selector, bool trackChanges)
        {
            try
            {
                return !trackChanges
                    ? await Task.Run(() => _context.Set<T>().AsNoTracking().Select(selector))
                    : await Task.Run(() => _context.Set<T>().Select(selector));
            }
            catch (Exception ex)
            {
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets selected columns and applies filter on them from a table based upon what selector is passed
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public async Task<IQueryable<TResult>> GetSelectedColumnsAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> filter, bool trackChanges)
        {
            return !trackChanges
                ? await Task.Run(() => _context.Set<T>().AsNoTracking().Where(filter).Select(selector))
                : await Task.Run(() => _context.Set<T>().Where(filter).Select(selector));
        }

        /// <summary>
        /// Gets selected columns and applies pagination on them from a table based upon what selector is passed
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="trackChanges"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        public async Task<IQueryable<TResult>> GetSelectedColumnsWithPaginationAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            bool trackChanges,
            int pageNumber,
            int pageSize)
        {
            IQueryable<T> query = !trackChanges
                ? _context.Set<T>().AsNoTracking()
                : _context.Set<T>();

            return await Task.Run(() => query
                .Select(selector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }
        /// <summary>
        /// Gets selected columns and applies pagination and filter on them from a table based upon what selector is passed
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="filter"></param>
        /// <param name="trackChanges"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IQueryable<TResult>> GetSelectedColumnsWithPaginationAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> filter,
            bool trackChanges,
            int pageNumber,
            int pageSize)
        {
            IQueryable<T> query = !trackChanges
                ? _context.Set<T>().AsNoTracking().Where(filter)
                : _context.Set<T>().Where(filter);

            return await Task.Run(() => query
                .Select(selector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }

        /// <summary>
        /// return count of entities
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
        {
            return await _entities.CountAsync();
        }

        /// <summary>
        /// Get selected column, implements server side pagination and search
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="searchFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public async Task<IQueryable<TResult>> GetSelectedColumnsWithPaginationAndSearchAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> searchFilter, int pageNumber, int pageSize, bool trackChanges)
        {
            IQueryable<T> query = _context.Set<T>();

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            if (searchFilter != null)
            {
                query = query.Where(searchFilter);
            }

            return await Task.Run(() => query
                .Select(selector)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }
    }
}
