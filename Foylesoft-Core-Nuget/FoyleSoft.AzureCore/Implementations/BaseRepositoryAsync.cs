using FoyleSoft.AzureCore.Extensions;
using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.Core.Implementations.Data;
using FoyleSoft.Core.Implementations.Data.Tables.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class BaseRepositoryAsync<T> : IBaseRepositoryAsync<T> where T : class, IBaseTable
    {
        protected readonly DbContext _context;
        protected readonly ICacheService _cacheService;
        protected readonly DbContext _logContext;
        protected readonly ISessionUser _sessionUser;
        public event AfterSaveEventHandler AfterSave;
        protected readonly string _baseTableName;
        public BaseRepositoryAsync(DbContext context, ICacheService cacheService, DbContext logContext, ISessionUser sessionUser, string baseTableName = null)
        {
            _context = context;
            _cacheService = cacheService;
            _logContext = logContext;
            _sessionUser = sessionUser;
            _baseTableName = baseTableName;
            this.AfterSave += Repository_AfterSave;


        }
        public ChangeTracker ChangeTracker
        {
            get { return _context.ChangeTracker; }
        }
        public virtual async Task<T> AddAsync(T t)
        {
            await _context.Set<T>().AddAsync(t);
            return t;
        }

        public virtual async Task<List<T>> AddRangeAsync(List<T> tList)
        {
            await _context.Set<T>().AddRangeAsync(tList);
            return tList;
        }

        public virtual Task<int> CountAsync()
        {
            return _context.Set<T>().CountAsync();
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                if (entity is ISoftDelete softDeleteEntity)
                {
                    softDeleteEntity.IsDeleted = true;
                    await UpdateAsync(entity, entity.Id);
                }
                else
                {
                    _context.Set<T>().Remove(entity);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> DeleteRangeAsync(List<T> entityList)
        {
            try
            {
                if (entityList.Any() && entityList.FirstOrDefault() is ISoftDelete)
                {
                    foreach (T entity in entityList)
                    {
                        if (entity is ISoftDelete softDeleteEntity)
                        {
                            softDeleteEntity.IsDeleted = true;
                            await UpdateAsync(entity, entity.Id);
                        }
                    }
                }
                else
                {
                    _context.Set<T>().RemoveRange(entityList);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }
        private bool disposed = false;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<IQueryable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking().Where(predicate);
            return await Task.FromResult(query);
        }


        public virtual async Task<IQueryable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = await GetAllAsync();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return await Task.FromResult(queryable.AsNoTracking());
        }


        public virtual async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            List<T> result = await _context.Set<T>().Where(match).AsNoTracking().ToListAsync();
            return result;
        }


        public virtual async Task<T> GetAsync(int id)
        {
            try
            {
                T entity = await _context.Set<T>().FirstOrDefaultAsync(f=>f.Id == id);
                return entity;
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, such as logging or rethrowing
                throw;
            }
        }


        public virtual async Task<IQueryable<T>> GetAllAsync()
        {
            IQueryable<T> queryable = _context.Set<T>().AsNoTracking();
            return await Task.FromResult(queryable);
        }


        public async Task<T> GetAsync(
                     Expression<Func<T, bool>> predicate,
            bool trackEntities,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _context.Set<T>().Include(includeProperties);

            return trackEntities ?
               await query.FirstOrDefaultAsync(predicate) :
              await query.AsNoTracking().FirstOrDefaultAsync(predicate);
        }



        //public async Task<IQueryable<T>> GetAllAsync(
        //                        Expression<Func<T, bool>> predicate,
        //                        bool trackEntities = false,
        //                        params Expression<Func<T, object>>[] includeProperties)
        //{
        //    // Start with the base query for the entity type
        //    IQueryable<T> query = _context.Set<T>();

        //    // Include the specified related properties
        //    foreach (var includeProperty in includeProperties)
        //    {
        //        query = query.Include(includeProperty);
        //    }

        //    // Optionally disable entity tracking
        //    if (!trackEntities)
        //    {
        //        query = query.AsNoTracking();
        //    }

        //    // Apply the predicate filter to the query
        //    IQueryable<T> result = query.Where(predicate);

        //    return result;
        //}
        public async Task<IQueryable<T>> GetAllAsync(
           Expression<Func<T, bool>> predicate,
           bool trackEntities = false,
           params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _context.Set<T>().Include(includeProperties);

            return trackEntities ?
                query.Where(predicate) :
                query.AsNoTracking().Where(predicate);
        }

        
        public virtual async Task SaveAsync()
        {
            var entities = ChangeTracker.Entries()
                      .Where(p => p.State == EntityState.Modified || p.State == EntityState.Added || p.State == EntityState.Deleted)
                      .Select(f => (f.Entity, f.State)).ToList<(object Entity, EntityState state)>();

            await _context.SaveChangesAsync();
            if (AfterSave != null)
                AfterSave(_context, entities);
        }

        public virtual async Task<T> UpdateAsync(T t, int id)
        {
            if (t == null)
                return null;

            T exist = await _context.Set<T>().FindAsync(id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                await _context.SaveChangesAsync();
            }

            return exist;
        }


        public async Task<List<T>> FindCachedAllAsync(Expression<Func<T, bool>> match)
        {
            // Retrieve the cached dictionary of entities
            var cachedEntities = await _cacheService.GetCachedQueryDictionaryAsync<T>("Table_" + typeof(T).Name, _context.Set<T>().AsQueryable());

            // Filter the cached entities based on the predicate
            var matchedEntities = cachedEntities.Values.ToList().Where(match.Compile()).ToList();

            return matchedEntities;
        }



        public DbContext Context { get { return _context; } }

        protected virtual void Repository_AfterSave(DbContext context, List<(object Entity, EntityState state)> entities)
        {
            if (entities.Where(f => f.Entity is BaseTable).Any())
            {
                _cacheService.RemoveCachedStringAsync("Table_" + typeof(T).Name);
                if (_baseTableName != null)
                {
                    _cacheService.RemoveCachedStringAsync("Table_" + _baseTableName);
                }
            };

            if (_sessionUser.CurrentUserId != 0) // do not log system movements..
            {
                try
                {
                    entities.Where(f => f.Entity is BaseTable).ToList()
                    .ForEach(f =>
                    {
                        var table = _logContext.Set<FcTable>().AsNoTracking().FirstOrDefault(g => g.TableName == typeof(T).Name);
                        if (table == null)
                        {
                            table = new FcTable
                            {
                                TableName = typeof(T).Name
                            };
                            _logContext.Set<FcTable>().Add(table);
                            _logContext.SaveChanges();
                        }
                        if (table != null)
                        {
                            string data = JsonConvert.SerializeObject(f.Entity);
                            var log = new FcTableLog
                            {

                                FcTableId = table.Id,
                                LogDate = DateTime.Now,
                                LogUserId = _sessionUser.CurrentUserId,
                                RawData = data,
                                State = f.state
                            };
                            _logContext.Set<FcTableLog>().Add(log);
                        }

                    });
                    _logContext.SaveChanges();
                }
                catch { }
            }
        }

        public Task<T> GetCachedAsync(int id)
        {
            return GetAsync(id);
        }

        public async Task<bool> AnyAsync<T>() where T : class
        {
            return await _context.Set<T>().AsNoTracking().AnyAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().AnyAsync(predicate);
        }
    }
}

