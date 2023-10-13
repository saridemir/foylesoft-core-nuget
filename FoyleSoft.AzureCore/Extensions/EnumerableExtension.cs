using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Extensions
{
    public static class EnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }

    }
}
