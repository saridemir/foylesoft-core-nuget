using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class DefaultTable : BaseTable
    {
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] public int CreaUserId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] public DateTimeOffset CreaDate { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] public int ModfUserId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore] public DateTimeOffset ModfDate { get; set; }
    }
}
