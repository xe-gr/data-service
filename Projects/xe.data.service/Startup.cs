using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using xe.data.service.Services;
using xe.data.service.Services.Interfaces;

namespace xe.data.service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfigurationReader, ConfigurationReader>();
            services.AddTransient<IDataCreator, DataCreator>();
            services.AddTransient<IDataRetriever, DataRetriever>();
            services.AddTransient<IDataService, DataService>();

            services.AddHealthChecks();

            services.AddRazorPages()
                .AddRazorPagesOptions(o =>
                {
                    o.Conventions.AuthorizeFolder("/");
                })
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Nothing to see here. Move along.");
                });
            });

            app.UseHealthChecks("/health");
        }
    }
}