using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetCoreMvcAngular.Repositories.Things;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Serilog.Core;
using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.Antiforgery;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMvcAngular
{
    public class Startup
    {
        public static LoggingLevelSwitch MyLoggingLevelSwitch { get; set; }

        public Startup(IHostingEnvironment env)
        {
            MyLoggingLevelSwitch = new LoggingLevelSwitch();
            MyLoggingLevelSwitch.MinimumLevel = LogEventLevel.Verbose;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(MyLoggingLevelSwitch)
                .Enrich.WithProperty("App", "AspNetCoreMvcAngular")
                .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.RollingFile("../Logs/AspNetCoreMvcAngular")
                .CreateLogger();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = "https://localhost:44348";
                options.RequireHttpsMetadata = true;
                options.ClientId = "angularmvcmixedclient";
                options.ClientSecret = "thingsscopeSecret";
                options.ResponseType = "code id_token";
                options.Scope.Add("thingsscope");
                options.Scope.Add("profile");
                options.SaveTokens = true;
            });

            // TODO add policies 
            services.AddAuthorization();

            services.AddSingleton<IThingsRepository, ThingsRepository>();
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IAntiforgery antiforgery)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            //Registered before static files to always set header
            app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());

            app.UseCsp(opts => opts
                .BlockAllMixedContent()
                .ScriptSources(s => s.Self()).ScriptSources(s => s.UnsafeEval())
                .StyleSources(s => s.UnsafeInline())
            );

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var angularRoutes = new[] {
                 "/default",
                 "/about"
             };

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //Registered after static files, to set headers for dynamic content.
            app.UseXfo(xfo => xfo.Deny());

            // Register this earlier if there's middleware that might redirect.
            // The IdentityServer4 port needs to be added here. 
            // If the IdentityServer4 runs on a different server, this configuration needs to be changed.
            app.UseRedirectValidation(t => t.AllowSameHostRedirectsToHttps(44348)); 

            app.UseXXssProtection(options => options.EnabledWithBlockMode());

            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;
                if (path != null && !path.ToLower().Contains("/api"))
                {
                    // XSRF-TOKEN used by angular in the $http if provided
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false, Secure = true });
                }

                if (context.Request.Path.HasValue && null != angularRoutes.FirstOrDefault(
                    (ar) => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.Path = new PathString("/");
                }

                await next();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });  
        }
    }
}

