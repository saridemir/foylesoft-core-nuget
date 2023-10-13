using FoyleSoft.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Models.Roles
{
    public class RoleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleTypes RoleType { get; set; }
        public string RoleTypeName { get; set; }
    }
}
