using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NorbitsChallenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Gir deg tilgang til all konfigurasjon i appen (appsettings.json, miljøvariabler etc.)
        public IConfiguration Configuration { get; }

        // Her registrerer du alle tjenester i DI-containeren (f.eks. DbContext, Repositories, etc.)
        public void ConfigureServices(IServiceCollection services)
        {
            // Om cookies
            services.Configure<CookiePolicyOptions>(options =>
            {
                // Bestemmer om brukeren må godta cookies. Her er det skrudd på (true).
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Legger Configuration-objektet tilgjengelig som en singleton
            services.AddSingleton<IConfiguration>(Configuration);

            // Her kan du registrere dine egne avhengigheter (Eksempel: Repositories, BLL osv.)
            SetupDependencyInjection(services);

            // Legger til støtte for MVC (Controllers + Views)
            // NB: Hvis du vil bruke Razor Pages, kan du også legge til services.AddRazorPages();
            services.AddControllersWithViews();
        }

        // Kalles av runtime. Konfigurerer request pipeline (HTTP).
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                // Viser feilsider med stack trace etc.
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Ved feil, bruk /Home/Error-siden
                app.UseExceptionHandler("/Home/Error");
                // Tving HTTPS ved produksjon
                app.UseHsts();
            }

            // Tvinger HTTP -> HTTPS-redirect
            app.UseHttpsRedirection();
            // Tillater bruk av statiske filer (wwwroot)
            app.UseStaticFiles();
            // Cookie Policy
            app.UseCookiePolicy();

            // Ruting
            app.UseRouting();

            // (Valgfritt) Hvis du har autorisasjon, legg til:
            // app.UseAuthorization();

            // Endpoints
            app.UseEndpoints(endpoints =>
            {
                // Standardrute: https://localhost:5001/{controller=Home}/{action=Index}/{id?}
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Evt. endpoints.MapRazorPages() hvis du bruker Razor Pages
            });
        }

        // Her kan du legge registrering av egne services i DI om du ønsker
        private void SetupDependencyInjection(IServiceCollection services)
        {
            // Eksempel:
            // services.AddTransient<IMyRepository, MyRepository>();
        }
    }
}
