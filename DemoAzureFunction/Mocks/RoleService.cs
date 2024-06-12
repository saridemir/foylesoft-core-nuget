using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Interfaces.Repositories.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAzureFunction.Mocks
{
    public class RoleService : FoyleSoft.AzureCore.Implementations.RoleService, IRoleService
    {
        public RoleService(/*ICacheService cacheService,*/ //ILog logger, 
            ICoreRoleRepositoryAsync roleRepository, ICustomUserRoleRepositoryAsync customUserRoleRepository, ISessionUser sessionService, ICoreRoleMappingRepositoryAsync roleMappingRepository, ICoreUserRoleRepositoryAsync userRoleRepository)
            :base(roleRepository, customUserRoleRepository, sessionService, roleMappingRepository, userRoleRepository)
        {

        }
    }
}
