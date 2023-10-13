using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.Core.Implementations.Data;
using System;
using System.Collections.Generic;
using System.Text;


namespace FoyleSoft.Core.Implementations
{
    public class CustomUserRole : DefaultTable
    {
        public int CustomId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
        public virtual Role Role { get; set; }

    }
}
