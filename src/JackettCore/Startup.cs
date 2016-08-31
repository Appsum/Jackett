using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace JackettCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            // Serilog
            SerilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(AppLogLevel)
                .WriteTo.LiterateConsole()
                //.WriteTo.RollingFile("log-{Date}.log")
                .CreateLogger();

            // Set the standard logger to SerilogLogger
            Log.Logger = SerilogLogger;

            // Build configuration
            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        public IContainer AutofacContainer { get; private set; }

        private Serilog.ILogger SerilogLogger { get; set; }
        private LoggingLevelSwitch AppLogLevel { get; set; } = new LoggingLevelSwitch(LogEventLevel.Verbose);

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddIdentity();

            // Add framework services.
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Never",
                    new CacheProfile
                    {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            });

            var logger = new SerilogLoggerProvider(SerilogLogger).CreateLogger("Main");

            AutofacContainer = AutofacLoader.Configure(services, logger).Build();

            return new AutofacServiceProvider(AutofacContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            // If you want to dispose of resources that have been resolved in the
            // application container, register for the "ApplicationStopped" event.
            applicationLifetime.ApplicationStopped.Register(() => AutofacContainer.Dispose());
        }
    }
}
