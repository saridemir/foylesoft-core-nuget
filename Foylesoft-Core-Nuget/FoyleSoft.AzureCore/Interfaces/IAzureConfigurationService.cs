using FoyleSoft.AzureCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IAzureConfigurationService
    {
        AzureConfigInfo AzureConfig { get; set; }

        ClientConfigInfo ClientConfig { get; set; }
    }
}
