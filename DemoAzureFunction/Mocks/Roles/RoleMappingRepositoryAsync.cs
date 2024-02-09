using DemoAzureFunction.Mocks;
using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Interfaces.Repositories.Roles;
using FoyleSoft.Core.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvideBackend.Common.Data.Repositories.Roles
{
    public class RoleMappingRepositoryAsync : BaseRepositoryAsync<RoleMapping>, IRoleMappingRepositoryAsync
    {
        public RoleMappingRepositoryAsync(MockContext context, ICacheService cacheService, MockLogContext logContext, ISessionUser sessionUser, string baseTableName = null) : base(context, cacheService, logContext, sessionUser, baseTableName)
        {
        }
    }
}
