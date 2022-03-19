using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Specialized;
using IdentityServer4.Extensions;
using System.Collections.Generic;

namespace TheWorkBook.Identity
{
    public static class StringExtensions
    {
        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url != null)
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    url = url.Substring(idx + 1);
                }
                var query = QueryHelpers.ParseNullableQuery(url);
                if (query != null)
                {
                    return query.AsNameValueCollection();
                }
            }

            return new NameValueCollection();
        }

        public static NameValueCollection FromFullDictionary(this IDictionary<string, string[]> source)
        {
            var nvc = new NameValueCollection();

            foreach ((string key, string[] strings) in source)
            {
                foreach (var value in strings)
                {
                    nvc.Add(key, value);
                }
            }

            return nvc;
        }
    }
}
