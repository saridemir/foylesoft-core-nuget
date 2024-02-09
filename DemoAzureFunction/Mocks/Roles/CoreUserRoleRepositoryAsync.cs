using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces.Repositories.Roles;
using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.Core.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoAzureFunction.Mocks;

namespace EvideBackend.Common.Data.Repositories.Roles
{

    public class CoreUserRoleRepositoryAsync : BaseRepositoryAsync<UserRole>, ICoreUserRoleRepositoryAsync
    {
        public CoreUserRoleRepositoryAsync(MockContext context, ICacheService cacheService, MockLogContext logContext, IMockSessionService sessionUser, string baseTableName = null) : base(context, cacheService, logContext, sessionUser, baseTableName)
        {
        }
    }
}
