using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    /// <summary>
    /// TODO: bu kısmı iyileştirmek gerekiyor
    /// </summary>
    public interface ISessionUser
    {
        int CurrentUserId { get; }
        //int CurrentCompanyId { get; }

        //int CurrentStoreId { get; }
    }
}
