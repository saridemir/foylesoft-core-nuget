using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoyleSoft.AzureCore.Models;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IGraphApiService
    {
        Task<string> CreateUserAsync(AddGrapUserInfo userInfo);
        Task<string> ReadUserAsync(string userGuid);
        Task<string> UpdateUserAsync(string userGuid, UpdateGrapUserInfo userInfo);
        Task DeleteUserAsync(string userGuid);
    }

}
