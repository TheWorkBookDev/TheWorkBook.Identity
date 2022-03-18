using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;
using TheWorkBook.Utils;
using TheWorkBook.Utils.Abstraction;
using TheWorkBook.Utils.Abstraction.ParameterStore;

namespace TheWorkBook.Identity
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        private readonly IEnvVariableHelper _envVariableHelper;
        readonly bool traceEnabled = false;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;

            _envVariableHelper = new TheWorkBook.Utils.EnvVariableHelper(null, Configuration);

            string loggingLevel = _envVariableHelper.GetVariable("Logging__LogLevel__Default");
            if (!string.IsNullOrWhiteSpace(loggingLevel))
                traceEnabled = loggingLevel.Equals("Trace", StringComparison.InvariantCultureIgnoreCase);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .SetApplicationName("TheWorkBook")
                .PersistKeysToAWSSystemsManager($"/DataProtection");

            services.AddControllersWithViews();
            services.AddTransient<IEnvVariableHelper, EnvVariableHelper>();

            using IParameterStore parameterStore = GetParameterStore();

            LogTrace("Got IParameterStore object");

            IParameter connectionStringParam = parameterStore.GetParameter("/database/app-connection-string");

            LogTrace("Got connectionStringParam object");

            var connectionString = connectionStringParam.Value;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                .AddTestUsers(TestUsers.Users)
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                });

            if (Environment.IsDevelopment())
            {
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                IParameterList parameters = parameterStore.GetParameterListByPath("/identity/signingcert/");

                string certificate = AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar
                    + "SigningCertificate"
                    + System.IO.Path.DirectorySeparatorChar
                    + parameters.GetParameterValue("certname");

                var cert = new X509Certificate2(
                  certificate,
                  parameters.GetParameterValue("password"),
                  X509KeyStorageFlags.MachineKeySet |
                  X509KeyStorageFlags.PersistKeySet |
                  X509KeyStorageFlags.Exportable
                );

                builder.AddSigningCredential(cert);
            }

            services.AddAuthentication();
                //.AddGoogle(options =>
                //{
                //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                //    // register your IdentityServer with Google at https://console.developers.google.com
                //    // enable the Google+ API
                //    // set the redirect URI to https://localhost:5001/signin-google
                //    options.ClientId = "copy client ID from Google here";
                //    options.ClientSecret = "copy client secret from Google here";
                //});
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private IParameterStore GetParameterStore()
        {
            LogTrace("Entered GetParameterStore()");

            bool useSpecifiedParamStoreCreds =
                _envVariableHelper.GetVariable("UseSpecifiedParamStoreCreds") != null
                    && _envVariableHelper.GetVariable("UseSpecifiedParamStoreCreds")
                        .Equals("true", StringComparison.InvariantCultureIgnoreCase);

            LogTrace($"useSpecifiedParamStoreCreds: {useSpecifiedParamStoreCreds}");

            AmazonSimpleSystemsManagementClient client;

            if (useSpecifiedParamStoreCreds)
            {
                Amazon.RegionEndpoint regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_envVariableHelper.GetVariable("AWSRegion", true));

                LogTrace($"instantiating AmazonSimpleSystemsManagementClient: {useSpecifiedParamStoreCreds}");

                client = new AmazonSimpleSystemsManagementClient(_envVariableHelper.GetVariable("ParamStoreConnectionKey", true),
                    _envVariableHelper.GetVariable("ParamStoreConnectionSecret", true), regionEndpoint);
            }
            else
            {
                LogTrace($"instantiating default AmazonSimpleSystemsManagementClient: {useSpecifiedParamStoreCreds}");
                client = new AmazonSimpleSystemsManagementClient();
            }

            LogTrace("'AmazonSimpleSystemsManagementClient' client instantiated");

            return new TheWorkBook.Utils.ParameterStore.ParameterStore(client);
        }

        private void LogTrace(string logMessage)
        {
            if (traceEnabled)
            {
                LambdaLogger.Log(logMessage);
            }
        }
    }
}
