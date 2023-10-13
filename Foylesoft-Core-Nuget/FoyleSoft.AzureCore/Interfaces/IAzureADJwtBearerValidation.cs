using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IAzureADJwtBearerValidation
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(string authorizationHeader);        
    }
}
