using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IBaseResponse<T>
    {
        bool IsSuccess { get; set; }
        string ErrorMessage { get; set; }
        T Data { get; set; }
        IBaseResponse<K> ConvertError<K>();
    }
}
