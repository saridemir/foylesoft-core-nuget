using FoyleSoft.AzureCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DemoAzureFunction.Mocks
{
    
    public interface IMockSessionBaseRepositoryAsync<T> : ISessionBaseRepositoryAsync<T>
       where T : class
    {

       // Task<IQueryable<T>> FindByLicenseAsync(Expression<Func<T, bool>> predicate);
      //  Task<T> GetByLicenseAsync(int id);
    }
}
