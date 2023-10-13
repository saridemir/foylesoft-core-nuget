//using FoyleSoft.AzureCore.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
//using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
//using Microsoft.Extensions.Logging;
//using Microsoft.OpenApi.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FoyleSoft.AzureCore.Implementations
//{
//    [FoyleSoftAzureAuthorization]
//    public class SessionBaseAzureFunction<T> where T : class        
//    {
//        protected readonly ILogger<T> _logger;
//        protected readonly ICacheService _cacheService;
//        public SessionBaseAzureFunction(
//            ILogger<T> log, ICacheService cacheService)
//        {
//            _logger = log;
//            _cacheService = cacheService;
//        }
//        [FunctionName("FunctionName")]
//        [OpenApiOperation(operationId: "Run", tags: new[] { "User" }, Summary = "GetUserInfoByUserId", Description = "",
//            Visibility = OpenApiVisibilityType.Important)]
//        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
//        public virtual async Task<IActionResult> Run(
//            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Yilmaz")] HttpRequest req)
//        {
//            return await Task.FromResult(new OkObjectResult(""));
//        }
//    }
//}
