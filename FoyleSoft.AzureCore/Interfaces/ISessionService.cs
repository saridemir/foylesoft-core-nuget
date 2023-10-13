using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface ISessionService : ISessionUser
    {
        //string CurrentUserName { get; }
        string CurrentName { get; }
        string CurrentSurname { get; }
        string CurrentImageUrl { get; }
        
        string Token { get; }

        //string ClassJson { get; }
        DateTimeOffset GetExpiryTimestamp();
    }
}
