using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;

namespace CodeCamp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            
            var builder = new ConfigurationBuilder()
                // read config file
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            // read env vars
            builder.AddEnvironmentVariables();
            _config = builder.Build();
        }

        private IConfigurationRoot _config;

        // This method gets called by the runtime. Use this method to add services to the container.
        // setup dependency injection layer
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            // Singleton: get service once for lifetime of application
            services.AddSingleton(_config);

            // Scoped: get service once per request
            // AddDbContext is from EF. Need to register the context with DI.
            services.AddDbContext<CampContext>(ServiceLifetime.Scoped);
            // pass in interface and concrete implementation
            services.AddScoped<ICampRepository, CampRepository>();

            services.AddApplicationInsightsTelemetry(_config);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // setup how requests are being handled.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_config.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
        }
    }
}
