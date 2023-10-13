using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface ISessionBaseRepositoryAsync<T> : IBaseRepositoryAsync<T>, IDisposable
       where T : class
    {
        //Task<IQueryable<T>> FindByCompanyAsync(Expression<Func<T, bool>> predicate);
        //Task<T> GetByCompanyAsync(int id);
        Task<IQueryable<T>> GetWithPagingAsync(int pageNumber, int itemCount, Expression<Func<T, bool>> predicate);
        Task<int> GetPagingListCountAsync(Expression<Func<T, bool>> predicate);
    }
}
