
using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.Core.Implementations.Data;

namespace FoyleSoft.Core.Implementations
{
    public class UserRole : DefaultTable
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        //[System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] public User User { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] 
        public virtual Role Role { get; set; }
    }
}