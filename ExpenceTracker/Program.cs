using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using ExpenceTracker.Resources; // TBD later, but safe to add
using Microsoft.Extensions.Configuration;

using ExpenceTracker.Modules.Badges.Domain;
using ExpenceTracker.Modules.Badges.Infrastructure;
using ExpenceTracker.Modules.Expenses.Domain;
using ExpenceTracker.Modules.Expenses.Infrastructure;

namespace ExpenceTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Fetch Persistence Configuration
            var dataDirectory = builder.Configuration["Persistence:DataDirectory"] ?? "data";
            var provider = builder.Configuration["Persistence:Provider"] ?? "JsonFile";

            if (provider == "JsonFile")
            {
                builder.Services.AddScoped<IBadgeRepository, JsonBadgeRepository>();
                builder.Services.AddScoped<IExpenseRepository, JsonExpenseRepository>();
            }
            // Add services to the container.

            // Add services to the container.
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en") };
                options.DefaultRequestCulture = new RequestCulture("ar");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            builder.Services.AddRazorPages()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
