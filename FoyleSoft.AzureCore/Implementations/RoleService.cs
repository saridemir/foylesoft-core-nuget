using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Interfaces.Repositories.Roles;
using FoyleSoft.AzureCore.Models;
using FoyleSoft.AzureCore.Models.Roles;
using FoyleSoft.Core.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace FoyleSoft.AzureCore.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly ISessionUser _sessionService;
        private readonly ICoreRoleRepositoryAsync _roleRepository;
        private readonly ICoreRoleMappingRepositoryAsync _roleMappingRepository;
        private readonly ICoreUserRoleRepositoryAsync _userRoleRepository;
        private readonly ICustomUserRoleRepositoryAsync _customUserRoleRepository;

        //private readonly ILog _logger;

        public RoleService(/*ICacheService cacheService,*/ //ILog logger, 
            ICoreRoleRepositoryAsync roleRepository, ICustomUserRoleRepositoryAsync customUserRoleRepository, ISessionUser sessionService, ICoreRoleMappingRepositoryAsync roleMappingRepository, ICoreUserRoleRepositoryAsync userRoleRepository)
        {
            _sessionService = sessionService;
            _roleRepository = roleRepository;
            _roleMappingRepository = roleMappingRepository;
            _userRoleRepository = userRoleRepository;
            _customUserRoleRepository = customUserRoleRepository;
        }


        public virtual async Task<IBaseResponse<List<string>>> GetPermitedControllerMethods(string controllerName, List<string> methods)
        {
            var roleMappings = await _roleMappingRepository.FindCachedAllAsync(f => true);

            var userRoleIds = _userRoleRepository.FindCachedAllAsync(f => f.UserId == _sessionService.CurrentUserId).Result
                .Select(y => y.RoleId)
                .Distinct()
                .ToList()
            .Union(_customUserRoleRepository.FindCachedAllAsync(f => f.UserId == _sessionService.CurrentUserId).Result.Select(f => f.RoleId).Distinct())
            .ToList();

            var userRoleMappings = roleMappings
                .Where(f => userRoleIds.Contains(f.RoleId) && (f.RolePattern.IndexOf(controllerName+".") >= 0  || f.RolePattern=="*.*"))
                    .Select(f => f.RolePattern).ToList();
            var allUserRoleMappings = userRoleMappings.Where(p => p.EndsWith(".*")).Select(f => f.Replace("*", "")).ToList();
            var mets = methods.Where(f => userRoleMappings.Contains("*.*") || userRoleMappings.Contains(f) || allUserRoleMappings.Contains(f.Split('.')[0]+".")).Distinct()
                .ToList();
            var result =new List<string>();
            foreach (var f in mets)
            {
                var hasAccess = await HasAccessAsync(f);
                if (hasAccess)
                    result.Add(f);
            }

            return new BaseResponse<List<string>> { IsSuccess = true, Data = result.ToList() };
        }

        public virtual async Task<IBaseResponse<List<string>>> GetPermitedControllers(List<string> controlNames)
        {
            var userRoleIds = _userRoleRepository.FindCachedAllAsync(f => f.UserId == _sessionService.CurrentUserId).Result.Select(y => y.RoleId)
                .Distinct()
                .ToList()
            .Union(_customUserRoleRepository.FindByAsync(f => f.UserId == _sessionService.CurrentUserId).Result.Select(f => f.RoleId)
            .Distinct().ToList())
            .ToList();
            try
            {
                var roleMappings = await _roleMappingRepository.GetAllAsync().Result.Where(f => userRoleIds.Contains(f.RoleId)).ToListAsync();

                var result = new List<string>();
                if (roleMappings.Any(f => f.RolePattern == $"*.*"))
                {
                    controlNames.ForEach(controllerName =>
                    {
                        result.Add(controllerName);
                    });
                    return new BaseResponse<List<string>> { IsSuccess = true, Data = result };
                }
                else
                {
                    controlNames.ForEach(controllerName =>
                    {
                        if (roleMappings.Any(r => r.RolePattern.ToLower().Trim() == $"{controllerName}controller".ToLower()))
                        {
                            result.Add(controllerName);
                        }
                    });
                }

                return new BaseResponse<List<string>> { IsSuccess = true, Data = result };
            }
            catch (Exception _e)
            {
                throw _e;
            }
        }

        public virtual async Task<IBaseResponse<List<RoleInfo>>> GetRoles()
        {
            try
            {
                var roles = _roleRepository.FindCachedAllAsync(f => true).Result.ToList();
                List<Role> availableRoles = new List<Role>();
                if (await HasAccessAsync("*.*"))
                {
                    availableRoles = roles;
                }
                else if (await HasAccessAsync("ProjectAdmin"))
                {
                    roles.ForEach(async role =>
                    {
                        if (await HasAccessAsync("ProjectRole_" + role.Name))
                        {
                            availableRoles.Add(role);
                        }
                    });
                }
                else if (await HasAccessAsync("LicenseAdmin"))
                {
                    roles.ForEach(async role =>
                    {
                        if (await HasAccessAsync("Role_" + role.Name))
                        {
                            availableRoles.Add(role);
                        }
                    });
                }


                var infoList = availableRoles.Select(f => new RoleInfo
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    RoleType = f.RoleType,
                    RoleTypeName = f.RoleType.ToString()
                }).ToList();

                return new BaseResponse<List<RoleInfo>> { IsSuccess = true, Data = infoList };
            }
            catch (System.Exception ex)
            {
                return new BaseResponse<List<RoleInfo>> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        public virtual async Task<IBaseResponse<List<string>>> GetUserRoleMappings(int userId) 
        {
            var customRoles = await _customUserRoleRepository.GetAllIncludingAsync(f => f.Role).Result
            .Where(f => f.UserId == userId)
            .Select(f => f.Role.Id)
            .ToListAsync();

            var userRoles = await _userRoleRepository
                    .GetAllIncludingAsync(f => f.Role).Result
                    .Where(f => f.UserId == userId)
                    .Select(f => f.Role.Id).ToListAsync();

            var responseValues = customRoles.Union(userRoles).ToList();
            var result =await _roleMappingRepository.GetAllAsync(f => responseValues.Contains(f.RoleId));
            return new BaseResponse<List<string>> { IsSuccess = true, Data = result.Select(f=>f.RolePattern).ToList() };

        }
        public virtual async Task<IBaseResponse<List<string>>> GetUserRoles(int userId)
        {
            var customRoles = await _customUserRoleRepository.GetAllIncludingAsync(f => f.Role).Result
            .Where(f => f.UserId == userId)
            .Select(f => f.Role.Name)
            .ToListAsync();

            var userRoles = await _userRoleRepository
                    .GetAllIncludingAsync(f => f.Role).Result
                    .Where(f => f.UserId == userId)
                    .Select(f => f.Role.Name).ToListAsync();

            var responseValues = customRoles.Union(userRoles).ToList();

            return new BaseResponse<List<string>> { IsSuccess = true, Data = responseValues };
        }


        public virtual async Task<bool> HasAccessAsync(string key)
        {
            var roleMappings = await _roleMappingRepository.FindCachedAllAsync(f => true);
            var userRoleIds = _userRoleRepository.FindCachedAllAsync(f => f.UserId == _sessionService.CurrentUserId)
                .Result.Select(y => y.RoleId).Distinct().ToList();

            userRoleIds.AddRange(_customUserRoleRepository.FindByAsync(f => f.UserId == _sessionService.CurrentUserId).Result.Select(f => f.RoleId).ToList());

            var userRoleMappings = roleMappings.Where(f => userRoleIds.Contains(f.RoleId)).ToList();
            var split = key.Split(".");
            if (userRoleMappings.Any(p => (p.RolePattern == key)
                || (key.IndexOf(".") >= 0 && p.RolePattern == $"{split[0]}.*"
                    && split[1] != $"Admin")
                || (p.RolePattern == $"*.*"
                    && key.IndexOf(".") >= 0))
                )
            {
                return true;
            }
            return false;
        }

        public virtual async Task<IBaseResponse<Role>> SaveRole(Role role)
        {
            try
            {
                if (role.Id == 0)
                {
                    role = await _roleRepository.AddAsync(role);
                }
                else
                {
                    var roleDb = await _roleRepository.GetAsync(role.Id);
                    if (roleDb == null)
                        return new BaseResponse<Role>() { IsSuccess = false, ErrorMessage = "Role not found." };

                    role = await _roleRepository.UpdateAsync(role, role.Id);
                }

                var resSave = _roleRepository.SaveAsync();
                resSave.Wait();

                return new BaseResponse<Role> { IsSuccess = true, Data = role };
            }
            catch (System.Exception ex)
            {
                return new BaseResponse<Role> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
