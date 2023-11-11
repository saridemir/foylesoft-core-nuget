using FoyleSoft.AzureCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{

    public class AnonymousAzureFunction<T> where T : class, IAnonymousAzureFunction
    {
        protected readonly ILogger<T> _logger;
        protected readonly ICacheService _cacheService;
        public AnonymousAzureFunction(
            ILogger<T> log, ICacheService cacheService)
        {
            _logger = log;
            _cacheService = cacheService;
        }


    }
}
