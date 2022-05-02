using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TheWorkBook.Identity
{
    /// <summary>
    /// This class can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public static class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
