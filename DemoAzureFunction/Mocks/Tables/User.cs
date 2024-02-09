using FoyleSoft.AzureCore.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAzureFunction.Mocks.Tables
{
    public class User : DefaultTable
    {
        public string EmailAddress { get; set; }
        //public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public int EvideLicenseId { get; set; }
        public int EvideProjectId { get; set; }


        public string UserGuid { get; set; }
    }
}
