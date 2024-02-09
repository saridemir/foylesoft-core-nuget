using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IMailService : IBaseService
    {
        IBaseResponse<bool> SendMail(string mailAddress, string subject, string messageBody, bool isHtml);
    }
}
