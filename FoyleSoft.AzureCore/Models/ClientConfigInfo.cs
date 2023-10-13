using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Models
{
    public class ClientConfigInfo
    {
        public string EvideClientBaseUrl { get; set; }
        public string ClientSecretKey { get; set; }
        public string ClientRoleMappings { get; set; }
        public string ClientValidIssuer { get; set; }
        public string ClientValidAudience { get; set; }

        public string BuilderFormConstantTypeName { get; set; }
    }
}
