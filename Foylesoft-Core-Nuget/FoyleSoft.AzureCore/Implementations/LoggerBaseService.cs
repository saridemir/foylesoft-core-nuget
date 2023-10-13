using FoyleSoft.AzureCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public abstract class LoggerBaseService<T> : IBaseService
    {
        public LoggerBaseService(ILogger<T> logger)
        {
            Logger = logger;
        }
        public ILogger<T> Logger { get; private set; }
    }
    public abstract class LoggerBaseService: IBaseService
    {
        public LoggerBaseService(ILogger logger)
        {
            Logger = logger;
        }
        public ILogger Logger { get; private set; }
    }
}
