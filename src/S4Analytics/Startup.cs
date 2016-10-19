using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Lib.Identity;
using Lib.Identity.Models;
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
            // Add MVC.
            services.AddMvc(mvcOptions =>
            {
                // Return API exceptions as JSON, not HTML.
                mvcOptions.Filters.Add(new CustomJsonExceptionFilter(_env));
            });

            // Serialize enums to JSON as strings, rather than integers.
            services.Configure<MvcJsonOptions>(jsonOptions =>
            {
                jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            // Do not redirect to login for unauthorized API call; return Unauthorized status code instead.
            // http://stackoverflow.com/questions/34770886/mvc6-unauthorized-results-in-redirect-instead
            services.Configure<IdentityOptions>(identityOptions =>
            {
                identityOptions.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        var isApiCall = ctx.Request.Path.StartsWithSegments("/api");
                        var hasOkStatus = ctx.Response.StatusCode == (int)HttpStatusCode.OK;
                        if (isApiCall && hasOkStatus)
                        {
                            // todo: return HttpStatusCode.Forbidden when appropriate
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult<object>(null);
                    }
                };
            });

            // Add and configure Oracle user store.
            services.AddSingleton<IUserStore<S4IdentityUser>>(provider => {
                var options = provider.GetService<IOptions<ServerOptions>>();
                var connStr = options.Value.WarehouseConnStr;
                return new S4UserStore<S4IdentityUser>("S4_Analytics", connStr, null);
            });

            // Add and configure Oracle role store.
            services.AddSingleton<IRoleStore<S4UserRole>, S4RoleStore<S4UserRole>>();

            // Configure sign-in scheme to use cookies.
            services.AddAuthentication(authOptions =>
            {
                authOptions.SignInScheme = new IdentityCookieOptions().ExternalCookieAuthenticationScheme;
            });

            // Add identity services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IdentityMarkerService>();
            services.AddSingleton<IUserValidator<S4IdentityUser>, UserValidator<S4IdentityUser>>();
            services.AddSingleton<IPasswordValidator<S4IdentityUser>, PasswordValidator<S4IdentityUser>>();
            services.AddSingleton<IPasswordHasher<S4IdentityUser>, PasswordHasher<S4IdentityUser>>();
            services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddSingleton<IdentityErrorDescriber>();
            services.AddSingleton<ISecurityStampValidator, SecurityStampValidator<S4IdentityUser>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<S4IdentityUser>, UserClaimsPrincipalFactory<S4IdentityUser>>();
            services.AddSingleton<UserManager<S4IdentityUser>>();
            services.AddSingleton<RoleManager<S4UserRole>>();
            services.AddScoped<SignInManager<S4IdentityUser>>();

            // Add options.
            services.AddOptions();
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

        private class UserClaimsPrincipalFactory<TUser> : IUserClaimsPrincipalFactory<TUser>
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

        private class CustomJsonExceptionFilter : ExceptionFilterAttribute
        {
            IHostingEnvironment _env;
            public CustomJsonExceptionFilter(IHostingEnvironment env)
            {
                _env = env;
            }

            private object GetDetailedSerializableException(Exception ex)
            {
                return new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    data = ex.Data,
                    helpLink = ex.HelpLink,
                    hResult = ex.HResult,
                    source = ex.Source,
                    innerException = ex.InnerException != null
                        ? GetDetailedSerializableException(ex.InnerException)
                        : null
                };
            }

            private object GetSimpleSerializableException(Exception ex)
            {
                return new
                {
                    message = ex.Message,
                    innerException = ex.InnerException != null
                            ? GetSimpleSerializableException(ex.InnerException)
                            : null
                };
            }

            // http://stackoverflow.com/questions/35245893/mvc-6-webapi-returning-html-error-page-instead-of-json-version-of-exception-obje
            public override void OnException(ExceptionContext context)
            {
                var isApiCall = context.HttpContext.Request.Path.StartsWithSegments("/api");
                if (isApiCall)
                {
                    var serializableException = _env.EnvironmentName == "Local"
                        ? GetDetailedSerializableException(context.Exception)
                        : GetSimpleSerializableException(context.Exception);
                    var jsonResult = new JsonResult(serializableException);
                    jsonResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Result = jsonResult;
                }
            }
        }
    }
}
