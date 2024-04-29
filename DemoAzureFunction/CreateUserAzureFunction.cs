using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DemoAzureFunction
{
    public class CreateUserAzureFunction : BaseAzureFunction<CreateUserAzureFunction>, IBaseAzureFunction
    {
        public const string ControllerName = "Mock";
        public const string ActionName = "CreateUserAzureFunction";
        private readonly IAzureConfigurationService _azureConfigurationService;
        private readonly IGraphApiService _graphApiService;
        private readonly IMailService _mailService;
        public CreateUserAzureFunction(IMailService mailService,IGraphApiService graphApiService,ILogger<CreateUserAzureFunction> log, ICacheService cacheService, IAzureConfigurationService azureConfigurationService) : base(log, cacheService)
        {
            _mailService = mailService;
            _graphApiService = graphApiService;
            _azureConfigurationService = azureConfigurationService;
        }

        [FunctionName(ActionName + "AzureFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { ControllerName }, Summary = "GetVersion", Description = "",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ControllerName + "/" + ActionName)] HttpRequest req)
        {
            //_mailService.SendMail("saridemir@gmail.com", "yilmaz", "deneme", true);
            //var graphResult = await _graphApiService.CreateUserAsync(new AddGrapUserInfo
            //{
            //    DisplayName = "yilmaz saridemir",
            //    AccountEnabled = true,
            //    GivenName = "yilmaz",
            //    Surname = "saridemir",
            //    Identities = new List<Identity>
            //            {
            //                new Identity
            //                {
            //                    Issuer =_azureConfigurationService.AzureConfig.Domain,
            //                    IssuerAssignedId ="demo@foylesoft.co.uk",
            //                    SignInType = "emailAddress"
            //                }
            //            },
            //    PasswordProfile = new PasswordProfile
            //    {
            //        ForceChangePasswordNextSignIn = false,
            //        Password = "Aa" + Guid.NewGuid().ToString() + ".",
            //    }
            //});
            //dynamic graphResultData = JsonConvert.DeserializeObject(graphResult);

            return await Task.FromResult(new OkObjectResult("Ver. 1.0"));
        }
    }
}

