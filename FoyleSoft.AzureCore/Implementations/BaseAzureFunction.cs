﻿using FoyleSoft.AzureCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    [FoyleSoftAzureAuthorization]
    public class BaseAzureFunction<T> : AnonymousAzureFunction<T> where T : class, IBaseAzureFunction
    {
        protected readonly ILogger<T> _logger;
        protected readonly ICacheService _cacheService;
        public BaseAzureFunction(
            ILogger<T> log, ICacheService cacheService):base(log,cacheService)
        {

        }


    }
}
