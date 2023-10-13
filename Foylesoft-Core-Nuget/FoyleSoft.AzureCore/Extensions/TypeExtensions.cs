using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetConstants(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }

        public static T GetConstantValue<T>(this Type type,string constantName) where T : class
        {
            var fieldInfo = GetConstants(type).FirstOrDefault(f=>f.Name==constantName);
            if (fieldInfo != null)
            {
                return fieldInfo.GetRawConstantValue() as T;
            }
            else
                return null;
        }
    }
}
