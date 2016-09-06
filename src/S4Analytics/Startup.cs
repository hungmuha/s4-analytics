using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using S4Analytics.Models;

namespace S4Analytics
{
    public class ServerOptions
    {
        public string WarehouseConnStr { get; set; }
        public string SpatialConnStr { get; set; }
    }

    public class ClientOptions
    {
        // Members of this class are exposed via REST API to the Angular app.
        // Don't include anything sensitive here, especially passwords.
        public string Version { get; set; }
        public string BaseUrl { get; set; }
        public string SilverlightBaseUrl { get; set; }
    }

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddOptions();

            // Add configurations.
            services.Configure<ServerOptions>(serverOptions =>
            {
                serverOptions.WarehouseConnStr = Configuration.GetConnectionString("Warehouse");
                serverOptions.SpatialConnStr = Configuration.GetConnectionString("Spatial");
            });
            services.Configure<ClientOptions>(clientOptions =>
            {
                clientOptions.Version = Configuration["Version"];
                clientOptions.BaseUrl = Configuration["BaseUrl"];
                clientOptions.SilverlightBaseUrl = Configuration["SilverlightBaseUrl"];
            });

            // Add repositories.
            services.AddSingleton<IPbcatPedRepository, PbcatPedRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.EnvironmentName == "Local")
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
