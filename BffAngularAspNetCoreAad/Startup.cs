using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BffAngularAspNetCoreAad.Repositories.Things;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace BffAngularAspNetCoreAad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration);

            services.AddSingleton<IThingsRepository, ThingsRepository>();
            services.AddAntiforgery(options =>
            {
                options.Cookie.HttpOnly = false;
                options.HeaderName = "X-XSRF-TOKEN";
            });

            services.AddControllersWithViews()
                .AddMicrosoftIdentityUI()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Registered before static files to always set header
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseCsp(opts => opts
                .BlockAllMixedContent()
                .ScriptSources(s => s.Self()).ScriptSources(s => s.UnsafeEval())
                .StyleSources(s => s.UnsafeInline())
            );

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var angularRoutes = new[] {
                 "/default",
                 "/about"
             };

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //Registered after static files, to set headers for dynamic content.
            app.UseXfo(xfo => xfo.Deny());
            //app.UseRedirectValidation(t => t.AllowSameHostRedirectsToHttps(44348));
            app.UseRedirectValidation(opts =>
            {
                opts.AllowedDestinations("https://login.microsoftonline.com/7ff95b15-dc21-4ba6-bc92-824856578fc1/oauth2/v2.0/authorize");
            });

            app.UseXXssProtection(options => options.EnabledWithBlockMode());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;
                if (path != null && !path.ToLower().Contains("/api"))
                {
                    // XSRF-TOKEN used by angular in the $http if provided
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false });
                }

                if (context.Request.Path.HasValue && null != angularRoutes.FirstOrDefault(
                    (ar) => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.Path = new PathString("/");
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

