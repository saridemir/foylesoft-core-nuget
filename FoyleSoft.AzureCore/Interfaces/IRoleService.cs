using FoyleSoft.AzureCore.Models.Roles;
using FoyleSoft.Core.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IRoleService : IBaseService
    {
        Task<bool> HasAccessAsync(string key);
        Task<IBaseResponse<List<string>>> GetPermitedControllers(List<string> controlNames);
        Task<IBaseResponse<List<string>>> GetPermitedControllerMethods(string controllerName, List<string> methods);
        Task<IBaseResponse<List<string>>> GetUserRoles(int userId);

        Task<IBaseResponse<List<RoleInfo>>> GetRoles();

        Task<IBaseResponse<Role>> SaveRole(Role role);
    }
}
