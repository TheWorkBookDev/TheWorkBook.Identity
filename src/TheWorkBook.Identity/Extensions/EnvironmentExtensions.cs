using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Specialized;
using IdentityServer4.Extensions;
using System.Collections.Generic;

namespace TheWorkBook.Identity
{
    public static class EnvironmentExtensions
    {
        public static bool IsLocal(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment("Local");
        }
    }
}
