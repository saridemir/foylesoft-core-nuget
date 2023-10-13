using FoyleSoft.AzureCore.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoyleSoft.Core.Implementations.Data.Tables
{
    public class SystemCompany : DefaultTable
    {
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; }
    }
}
