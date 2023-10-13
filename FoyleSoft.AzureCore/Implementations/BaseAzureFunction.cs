using FoyleSoft.AzureCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace FoyleSoft.AzureCore.Implementations
{
    [FoyleSoftAzureAuthorization]
    public class BaseAzureFunction<T> where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly ICacheService _cacheService;
        public BaseAzureFunction(
            ILogger<T> log, ICacheService cacheService)
        {
            _logger = log;
            _cacheService = cacheService;
        }


    }
}
