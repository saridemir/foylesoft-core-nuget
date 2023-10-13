using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Extensions
{
    public static class StringExtensions
    {
        public static string PurifyJson(this string text)
        {
            return text
                .Replace("\\", "")
                .Replace("\"[", "[")
                .Replace("]\"", "]")
            .Trim('"');
        }
        public static bool IsValidEmailAddress(this string s)
        {
            if (s.Length >= 256 || !Regex.IsMatch(s,
                  @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                  RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                return false;
            }
            return true;
        }
    }
}
