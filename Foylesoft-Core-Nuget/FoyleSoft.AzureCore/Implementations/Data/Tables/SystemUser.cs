using FoyleSoft.AzureCore.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoyleSoft.Core.Implementations.Data.Tables
{
    public class SystemUser : DefaultTable
    {
        public Guid UserGuid { get; set; }
        public string UserName { get; set; }
        public int CompanyId { get; set; }
    }
}
