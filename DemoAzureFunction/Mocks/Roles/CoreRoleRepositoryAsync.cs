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
    public class CoreRoleRepositoryAsync : BaseRepositoryAsync<Role>, ICoreRoleRepositoryAsync
    {
        public CoreRoleRepositoryAsync(MockContext context, ICacheService cacheService, MockLogContext logContext, IMockSessionService sessionUser, string baseTableName = null) : base(context, cacheService, logContext, sessionUser, baseTableName)
        {
        }
    }
}
