using FoyleSoft.AzureCore.Interfaces;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using System.Security.Claims;

namespace FoyleSoft.AzureCore.Implementations
{
    public class FoyleSoftAuthorizationAttribute:Attribute, IAsyncActionFilter
    {
        private const string AuthenticationControllerName = "Authentication";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var logger = context.HttpContext.RequestServices
            //                .GetRequiredService<ILog>();
            //logger.Debug("OnActionExecutionAsync");
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var controllerName = controllerActionDescriptor.ControllerName;
                if (controllerName == AuthenticationControllerName)
                {
                    await next();
                }
                else
                {
                    var req = context.HttpContext.Request;
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
                    if ((principal = azureADJwtBearerValidation.ValidateTokenAsync(authorizationHeader).Result) == null)
                    {
                        throw new UnauthorizedAccessException("Unable to find auth you");

                    }
                    var actionName = controllerActionDescriptor.ActionName;
                    var pattern = $"{controllerName}.{actionName}";

                    var roleService = services.GetRequiredService<IRoleService>();

                    var hasAccess = await roleService.HasAccessAsync(pattern);
                    if (!hasAccess)
                    {
                        throw new UnauthorizedAccessException("You have no access for this page");
                    }
                }
            }
           

            await next();
        }

    }
}
