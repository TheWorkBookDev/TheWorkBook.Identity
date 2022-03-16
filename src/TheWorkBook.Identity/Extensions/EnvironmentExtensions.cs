using Microsoft.Extensions.Hosting;

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
