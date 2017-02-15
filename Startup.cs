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
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

            _env = env;
        }

        private IConfigurationRoot _config;
        private IHostingEnvironment _env;

        // ConfigureServices and Configure are called once when server fires up.

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

            //seed database
            services.AddTransient<CampDbInitializer>();

            services.AddApplicationInsightsTelemetry(_config);

            services.AddMvc( opt =>
            {
                // thise filters are added MVC filters added to every controller.
                // will redirect all http to https
                if (!_env.IsProduction())
                {
                    // specify port so  port shows up  for redirects in  development 
                    opt.SslPort = 44352;
                }
                opt.Filters.Add(new RequireHttpsAttribute());
            })
                .AddJsonOptions(opt =>
                {
                    // ReferenceLoopHandling deals with models that references other models. 
                    // error - default; serializwer as much as possible for one  resource, then quit 
                    // ignore - skip circular references for all resources
                    // serialize - serialize circular loops
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // add IHttpContextAccessor for CampUrlResolver.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // add IMapper interface as injectable dependeny
            services.AddAutoMapper();

            services.AddCors(cfg =>
            {
                cfg.AddPolicy("demo", bldr =>
                {
                    bldr.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:8000");
                });

                cfg.AddPolicy("anyGet", bldr =>
                {
                    bldr.AllowAnyHeader()
                    .WithMethods("GET")
                    .AllowAnyOrigin();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // setup how requests are being handled.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            CampDbInitializer seeder)
        {
            loggerFactory.AddConsole(_config.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
 
            // Seed is async task. use Wait to make it synchronous.
            seeder.Seed().Wait();
        }
    }
}
