using System.Collections;
using System.Collections.Generic;
using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.Core.Enums;
using FoyleSoft.Core.Implementations.Data;

namespace FoyleSoft.Core.Implementations
{
    public class Role : DefaultTable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleTypes RoleType { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] 
        public virtual List<CustomUserRole> CustomUserRoles { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
        public virtual List<RoleMapping> RoleMappings { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
        public virtual List<UserRole> UserRoles { get; set; }

        
    }
}