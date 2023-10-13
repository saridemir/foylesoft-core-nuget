using System;
using System.Collections.Generic;
using System.Linq;

namespace FoyleSoft.AzureCore.Extensions
{
	public static class EnumFlagExtension<T> where T: Enum, IConvertible
    {
        public static long GetValueFromFlags(List<T> inputs)
        {
            return inputs.Sum(n => (long)(object)n);
        }

        public static List<T> GetFlagsFromValue(long inputType)
        {
            if (inputType == 0)
                return new List<T>();

            return Enum.GetValues(typeof(T)).Cast<T>()
                   .Where(m => ((T)(object)inputType).HasFlag(m)).ToList();
        }
    }
}

