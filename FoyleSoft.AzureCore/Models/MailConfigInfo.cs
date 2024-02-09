using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Models
{
    public class MailConfigInfo
    {
        public string MailUserName { get; set; }
        public string MailPassword { get; set; }
        public string MailServer { get; set; }
        public int MailServerPort { get; set; }
        public bool SupportSSL { get; set; }
    }
}
