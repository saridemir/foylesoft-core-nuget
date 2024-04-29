using FoyleSoft.AzureCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class FoyleSoftAzureAuthorizationAttribute : FunctionInvocationFilterAttribute, IFunctionInvocationFilter
    {

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {

            var req = executingContext.Arguments["req"] as HttpRequest;
            if (req == null || !req.Headers.ContainsKey("Authorization"))
            {
                throw new AuthenticationException("No Authorization header was present");
            }
            var services = req.HttpContext.RequestServices;
            var azureConfigService = services.GetRequiredService<IAzureConfigurationService>();
            if (azureConfigService == null)
            {
                throw new Exception("Unable to get instance IAzureConfigurationService");
            }
            var azureADJwtBearerValidation = services.GetRequiredService<IAzureADJwtBearerValidation>();
            if (azureADJwtBearerValidation == null)
            {
                throw new Exception("Unable to get instance IAzureADJwtBearerValidation");
            }
            if (!req.Headers.ContainsKey("Authorization"))
            {
                throw new AuthenticationException("Unable to find Authorization header");

            }
            string authorizationHeader = req.Headers["Authorization"];

            // Check if the value is empty.
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                throw new ArgumentNullException("Unable to find Authorization header");
            }
            while (authorizationHeader.StartsWith("Bearer"))
            {
                authorizationHeader = authorizationHeader.Substring(7);
            }
            ClaimsPrincipal principal; // This can be used for any claims
            //if ((principal = azureADJwtBearerValidation.ValidateTokenAsync(authorizationHeader).Result) == null)
            //{
            //    throw new UnauthorizedAccessException("Unable to find auth you");

            //}
            var pathFolders = req.Path.ToString().Substring(azureConfigService.AzureConfig.BaseApi.Length).Split('/');
            var pattern = "";
            if (pathFolders.Length < 2)
            {
                pattern = $"{pathFolders[0]}.{req.HttpContext.Request.Method.ToUpper()}";
            }
            else
            {
                int i;
                if (int.TryParse(pathFolders[1], out i))
                {
                    pattern = $"{pathFolders[0]}.{req.HttpContext.Request.Method.ToUpper()}";
                }
                else
                {
                    pattern = $"{pathFolders[0]}.{pathFolders[1]}";
                }
            }
            var roleService = services.GetRequiredService<IRoleService>();

            var hasAccess = await roleService.HasAccessAsync(pattern);
            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("You have no access for this page");
            }

            return;
        }
        /// <summary>
        ///     Post-execution filter.
        /// </summary>
        public Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            // Nothing.
            return Task.CompletedTask;
        }
    }
}