using System;
using System.Collections.Generic;
using System.Text;

namespace FoyleSoft.Core.Enums
{
    [Flags]
    public enum UserRoles
    {
       // [Description("User")]
        User = 1,
        // [Description("Company Admin")]
        CompanyAdmin = 2,
       // [Description("System Admin")]
        SystemAdmin = 4,
    }
}
