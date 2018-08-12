using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

namespace CityInfo.API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public static IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //Configuration = builder.Build(); Core 1.0

            Configuration = configuration;
           

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();/*.AddJsonOptions(x=> { (x.SerializerSettings.ContractResolver as DefaultContractResolver)
                                                   .NamingStrategy = null; })  set json propery first letter to Uppercase. */

            /* .AddXmlSerializerFormatters() returns xml if we have header Accept and value application/xml*/

            #if DEBUG
                        services.AddTransient<IMailService, LocalMailService>();
            #else
                        services.AddTransient<IMailService, CloudMailService>();
            #endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();

            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            app.UseStatusCodePages();
            app.UseMvc();
            //app.UseMvcWithDefaultRoute();
        }
    }
}
