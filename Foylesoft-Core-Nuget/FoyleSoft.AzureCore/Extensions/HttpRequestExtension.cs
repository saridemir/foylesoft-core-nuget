using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Extensions
{
    public static class HttpRequestExtension
    {
        public static T ToObject<T>(this HttpRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = streamReader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(requestBody);
        }
    }
}
