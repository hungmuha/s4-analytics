using Lib.Identity;
using Lib.Identity.Models;
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
using S4Analytics.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace S4Analytics
{
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
                // Include exception detail for local debugging only.
                var showExceptionDetail = _env.EnvironmentName == "Local";
                mvcOptions.Filters.Add(
                    new CustomJsonExceptionFilter(showExceptionDetail));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("any admin", policy => policy.RequireRole("global admin", "agency admin"));
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.CookieName = ".S4Analytics.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.CookieHttpOnly = true;
            });

            // Add options.
            services.AddOptions();
            services.Configure<ServerOptions>(Configuration.GetSection("App"));
            services.Configure<ClientOptions>(Configuration.GetSection("App"));

            // Configure identity.
            services.Configure<IdentityOptions>(identityOptions =>
            {
                identityOptions.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    // Do not redirect to /Login for unauthorized API call; return Unauthorized status code instead.
                    // http://stackoverflow.com/questions/34770886/mvc6-unauthorized-results-in-redirect-instead
                    OnRedirectToLogin = ctx =>
                    {
                        var isApiCall = ctx.Request.Path.StartsWithSegments("/api");
                        var hasOkStatus = ctx.Response.StatusCode == (int)HttpStatusCode.OK;
                        if (isApiCall && hasOkStatus)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult<object>(null);
                    },
                    // Do not redirect to /AccessDenied for forbidden API call; return Forbidden status code instead.
                    OnRedirectToAccessDenied = ctx =>
                    {
                        var isApiCall = ctx.Request.Path.StartsWithSegments("/api");
                        var hasOkStatus = ctx.Response.StatusCode == (int)HttpStatusCode.OK;
                        if (isApiCall && hasOkStatus)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult<object>(null);
                    }
                };
            });

            // Add and configure profile store.
            services.AddSingleton<IProfileStore<S4UserProfile>>(provider =>
            {
                var options = provider.GetService<IOptions<ServerOptions>>();
                return new S4UserProfileStore(options.Value.MembershipApplicationName, options.Value.WarehouseConnStr);
            });

            // Add and configure Oracle user store.
            services.AddSingleton<IUserStore<S4IdentityUser<S4UserProfile>>>(provider => {
                var options = provider.GetService<IOptions<ServerOptions>>();
                var profileStore = provider.GetService<IProfileStore<S4UserProfile>>();
                return new S4UserStore<S4IdentityUser<S4UserProfile>, S4UserProfile>(
                    options.Value.MembershipApplicationName,
                    options.Value.WarehouseConnStr,
                    options.Value.MembershipConnStr,
                    profileStore);
            });

            // Add and configure Oracle role store.
            services.AddSingleton<IRoleStore<S4IdentityRole>>(provider => {
                var options = provider.GetService<IOptions<ServerOptions>>();
                return new S4RoleStore<S4IdentityRole>(
                    options.Value.MembershipApplicationName,
                    options.Value.WarehouseConnStr);
            });

            // Configure sign-in scheme to use cookies.
            services.AddAuthentication(authOptions =>
            {
                authOptions.SignInScheme = new IdentityCookieOptions().ExternalCookieAuthenticationScheme;
            });

            // Add identity services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IdentityMarkerService>();
            services.AddScoped<IUserValidator<S4IdentityUser<S4UserProfile>>, UserValidator<S4IdentityUser<S4UserProfile>>>();
            services.AddScoped<IPasswordValidator<S4IdentityUser<S4UserProfile>>, PasswordValidator<S4IdentityUser<S4UserProfile>>>();
            services.AddScoped<IPasswordHasher<S4IdentityUser<S4UserProfile>>, S4PasswordHasher<S4IdentityUser<S4UserProfile>>>();
            services.AddScoped<ILookupNormalizer, S4LookupNormalizer>();
            services.AddScoped<IRoleValidator<S4IdentityRole>, RoleValidator<S4IdentityRole>>();
            services.AddScoped<IdentityErrorDescriber>();
            services.AddScoped<ISecurityStampValidator, SecurityStampValidator<S4IdentityUser<S4UserProfile>>>();
            services.AddScoped<IUserClaimsPrincipalFactory<S4IdentityUser<S4UserProfile>>, S4UserClaimsPrincipalFactory<S4IdentityUser<S4UserProfile>>>();
            services.AddScoped<UserManager<S4IdentityUser<S4UserProfile>>>();
            services.AddScoped<RoleManager<S4IdentityRole>>();
            services.AddScoped<SignInManager<S4IdentityUser<S4UserProfile>>>();

            // Add repositories.
            services.AddSingleton<INewUserRequestRepository, NewUserRequestRepository>();
            services.AddSingleton<ICrashRepository, CrashRepository>();
            services.AddSingleton<IViolationRepository, ViolationRepository>();
            services.AddSingleton<Html5Conduit>();
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

            app.UseSession();

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

        private class CustomJsonExceptionFilter : ExceptionFilterAttribute
        {
            bool _showExceptionDetail;

            public CustomJsonExceptionFilter(bool showExceptionDetail)
            {
                _showExceptionDetail = showExceptionDetail;
            }

            // http://stackoverflow.com/questions/35245893/mvc-6-webapi-returning-html-error-page-instead-of-json-version-of-exception-obje
            public override void OnException(ExceptionContext context)
            {
                var isApiCall = context.HttpContext.Request.Path.StartsWithSegments("/api");
                if (isApiCall)
                {
                    var serializableException = _showExceptionDetail
                        ? GetDetailedSerializableException(context.Exception)
                        : GetSimpleSerializableException(context.Exception);
                    var jsonResult = new JsonResult(serializableException);
                    jsonResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Result = jsonResult;
                }
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
        }
    }
}
