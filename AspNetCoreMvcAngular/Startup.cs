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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddSingleton<IThingsRepository, ThingsRepository>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = "https://localhost:44348",
                RequireHttpsMetadata = true,

                ClientId = "angularmvcmixedclient",
                ClientSecret = "thingsscopeSecret",

                ResponseType = "code id_token",
                Scope = { "openid", "profile", "thingsscope" },

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true
            });

            var angularRoutes = new[] {
                 "/default",
                 "/about"
             };

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && null != angularRoutes.FirstOrDefault(
                    (ar) => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.Path = new PathString("/");
                }

                await next();
            });

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
            app.UseRedirectValidation(t => t.AllowSameHostRedirectsToHttps(44348)); //Register this earlier if there's middleware that might redirect.
            app.UseXXssProtection(options => options.EnabledWithBlockMode());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });  
        }
    }
}

