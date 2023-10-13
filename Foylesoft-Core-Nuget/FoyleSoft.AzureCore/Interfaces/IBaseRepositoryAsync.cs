using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace FoyleSoft.AzureCore.Interfaces
{
    public delegate void AfterSaveEventHandler(DbContext context, List<(object Entity, EntityState state)> entities);
    public interface IBaseRepositoryAsync<T> : IDisposable
      where T : class
    {
        event AfterSaveEventHandler AfterSave;

        ChangeTracker ChangeTracker { get; }
        DbContext Context { get; }

        Task<T> AddAsync(T t);
        Task<List<T>> AddRangeAsync(List<T> tList);
        Task<int> CountAsync();
        Task<bool> DeleteAsync(T entity);

        Task<bool> DeleteRangeAsync(List<T> entityList);

        Task<IQueryable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<List<T>> FindCachedAllAsync(Expression<Func<T, bool>> match);
        Task<IQueryable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetAsync(int id);
        Task<T> GetCachedAsync(int id);
        Task<IQueryable<T>> GetAllAsync();

        /// <summary>
        /// Get entity which satisfies given condition, including specified related entities.
        /// </summary>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackEntities = false,
            params Expression<Func<T, object>>[] includeProperties);

        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool trackEntities = false,
            params Expression<Func<T, object>>[] includeProperties);

        Task SaveAsync();
        Task<T> UpdateAsync(T t, int id);
        Task<bool> AnyAsync<T>() where T : class;
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    }
}
