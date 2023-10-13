using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.Core.Implementations.Data;
using FoyleSoft.Core.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class SessionBaseRepositoryAsync<T> : BaseRepositoryAsync<T>, ISessionBaseRepositoryAsync<T> where T : DefaultTable
    {
        protected readonly IRoleService _roleService;
        public SessionBaseRepositoryAsync(DbContext context, ICacheService cacheService, ISessionUser sessionUser, IRoleService roleService, FoyleSoftLogContext logContext, string tableName = null) 
            : base(context, cacheService, logContext, sessionUser, tableName)
        {
            _roleService = roleService;
        }
        public override Task<T> AddAsync(T t)
        {
            if (t is DefaultTable && _sessionUser != null && _sessionUser.CurrentUserId != 0)
            {
                DateTime dt = DateTime.Now;
                ((DefaultTable)t).CreaUserId = _sessionUser.CurrentUserId;
                ((DefaultTable)t).CreaDate = new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, new TimeSpan(0, 0, 0));
                ((DefaultTable)t).ModfUserId = _sessionUser.CurrentUserId;
                ((DefaultTable)t).ModfDate = new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, new TimeSpan(0, 0, 0));
            }
            return base.AddAsync(t);
        }

        public override Task<List<T>> AddRangeAsync(List<T> tList)
        {
            tList.ForEach(n =>
            {
                if (n is DefaultTable && _sessionUser != null && _sessionUser.CurrentUserId != 0)
                {
                    DateTime dt = DateTime.Now;
                    ((DefaultTable)n).CreaUserId = _sessionUser.CurrentUserId;
                    ((DefaultTable)n).CreaDate = new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, new TimeSpan(0, 0, 0));
                    ((DefaultTable)n).ModfUserId = _sessionUser.CurrentUserId;
                    ((DefaultTable)n).ModfDate = new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, new TimeSpan(0, 0, 0));
                }
            });

            return base.AddRangeAsync(tList);
        }
        public override Task<T> UpdateAsync(T t, int id)
        {
            if (t is DefaultTable && _sessionUser != null && _sessionUser.CurrentUserId != 0)
            {
                DateTime dt = DateTime.Now;
                ((DefaultTable)t).ModfUserId = _sessionUser.CurrentUserId;
                ((DefaultTable)t).ModfDate = new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, new TimeSpan(0, 0, 0));
            }

            return base.UpdateAsync(t, id);
        }
        public override Task<bool> DeleteAsync(T entity)
        {
            return base.DeleteAsync(entity);
        }



        public async Task<IQueryable<T>> GetWithPagingAsync(int pageNumber, int itemCount, Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = await base.GetAllAsync(predicate);
            IQueryable<T> result = query.Skip((pageNumber - 1) * itemCount).Take(itemCount);
            return result;
        }

        public async Task<int> GetPagingListCountAsync(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = await base.GetAllAsync(predicate);
            int count = await query.CountAsync();
            return count;
        }

    }
}