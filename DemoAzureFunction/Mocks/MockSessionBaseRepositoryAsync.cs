using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DemoAzureFunction.Mocks
{
    public class MockSessionBaseRepositoryAsync<T> : SessionBaseRepositoryAsync<T>, IMockSessionBaseRepositoryAsync<T> where T : DefaultTable
    {
        public MockSessionBaseRepositoryAsync(DbContext context, ICacheService cacheService, IMockSessionService sessionUser, IRoleService roleService, FoyleSoftLogContext logContext, string tableName = null)
            : base(context, cacheService, sessionUser, roleService, logContext, tableName)
        {

        }
        
        
    }
}
