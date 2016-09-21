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
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using AspNetCore.Identity.Oracle;
using AspNetCore.Identity.Oracle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            _env = env;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddOptions();

            services.Configure<MvcJsonOptions>(jsonOptions =>
            {
                // Serialize enums as strings, rather than integers.
                jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            // add and configure Oracle user store
            services.AddSingleton<IUserStore<OracleIdentityUser>>(provider => {
                var options = provider.GetService<IOptions<ServerOptions>>();
                var connStr = options.Value.WarehouseConnStr;
                return new OracleUserStore<OracleIdentityUser>("S4_Analytics", connStr, null);
            });

            services.AddSingleton<IRoleStore<OracleUserRole>, OracleRoleStore<OracleUserRole>>();

            // configure sign-in scheme to use cookies
            services.AddAuthentication(options =>
            {
                options.SignInScheme = new IdentityCookieOptions().ExternalCookieAuthenticationScheme;
            });

            // Hosting doesn't add IHttpContextAccessor by default
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IdentityMarkerService>();
            services.AddSingleton<IUserValidator<OracleIdentityUser>, UserValidator<OracleIdentityUser>>();
            services.AddSingleton<IPasswordValidator<OracleIdentityUser>, PasswordValidator<OracleIdentityUser>>();
            services.AddSingleton<IPasswordHasher<OracleIdentityUser>, PasswordHasher<OracleIdentityUser>>();
            services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddSingleton<IdentityErrorDescriber>();
            services.AddSingleton<ISecurityStampValidator, SecurityStampValidator<OracleIdentityUser>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<OracleIdentityUser>, UserClaimsPrincipalFactory<OracleIdentityUser>>();
            services.AddSingleton<UserManager<OracleIdentityUser>>();
            services.AddSingleton<RoleManager<OracleUserRole>>();
            services.AddScoped<SignInManager<OracleIdentityUser>>();

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
            services.AddSingleton<IPbcatRepository, PbcatRepository>();
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

            app.UseIdentity();

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

        public class UserClaimsPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser>
            where TUser : class
        {
            public UserClaimsPrincipalFactory(
                UserManager<TUser> userManager,
                IOptions<IdentityOptions> optionsAccessor)
            {
                if (userManager == null)
                {
                    throw new ArgumentNullException(nameof(userManager));
                }
                if (optionsAccessor == null || optionsAccessor.Value == null)
                {
                    throw new ArgumentNullException(nameof(optionsAccessor));
                }

                UserManager = userManager;
                Options = optionsAccessor.Value;
            }

            public UserManager<TUser> UserManager { get; private set; }

            public IdentityOptions Options { get; private set; }

            public virtual async Task<ClaimsPrincipal> CreateAsync(TUser user)
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                var userId = await UserManager.GetUserIdAsync(user);
                var userName = await UserManager.GetUserNameAsync(user);
                var id = new ClaimsIdentity(Options.Cookies.ApplicationCookieAuthenticationScheme,
                    Options.ClaimsIdentity.UserNameClaimType,
                    Options.ClaimsIdentity.RoleClaimType);
                id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
                id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, userName));
                if (UserManager.SupportsUserSecurityStamp)
                {
                    id.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType,
                        await UserManager.GetSecurityStampAsync(user)));
                }
                if (UserManager.SupportsUserRole)
                {
                    var roles = await UserManager.GetRolesAsync(user);
                    foreach (var roleName in roles)
                    {
                        id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                    }
                }
                if (UserManager.SupportsUserClaim)
                {
                    id.AddClaims(await UserManager.GetClaimsAsync(user));
                }

                return new ClaimsPrincipal(id);
            }
        }
    }
}
