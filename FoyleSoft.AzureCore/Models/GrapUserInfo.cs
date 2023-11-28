using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Models
{
    
    public class AddGrapUserInfo
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public List<Identity> Identities { get; set; }
        public PasswordProfile PasswordProfile { get; set; }
        public bool AccountEnabled { get; set; }

    }
    public class UpdateGrapUserInfo
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
    }
    
    public class Identity
    {
        public string SignInType { get; set; }
        public string Issuer { get; set; }
        public string IssuerAssignedId { get; set; }
    }
    public class PasswordProfile
    {
        public bool ForceChangePasswordNextSignIn { get; set; }
        public string Password { get; set; }
    }


}
