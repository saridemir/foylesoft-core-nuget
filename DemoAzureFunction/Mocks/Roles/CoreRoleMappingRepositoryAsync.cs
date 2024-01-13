using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces.Repositories.Roles;
using FoyleSoft.AzureCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoyleSoft.Core.Implementations;
using DemoAzureFunction.Mocks;

namespace EvideBackend.Common.Data.Repositories.Roles
{
    public class CoreRoleMappingRepositoryAsync : BaseRepositoryAsync<RoleMapping>, ICoreRoleMappingRepositoryAsync
    {
        public CoreRoleMappingRepositoryAsync(MockContext context, ICacheService cacheService, MockLogContext logContext, IMockSessionService sessionUser, string baseTableName = null) : base(context, cacheService, logContext, sessionUser, baseTableName)
        {
            
        }
    }
}
