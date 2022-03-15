using Microsoft.Extensions.Hosting;

namespace TheWorkBook.Identity.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsLocal(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment("Local");
        }
    }
}
