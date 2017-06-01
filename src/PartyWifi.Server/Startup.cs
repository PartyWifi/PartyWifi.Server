using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PartyWifi.Server.Components;
using PartyWifi.Server.DataModel;

namespace PartyWifi.Server
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            
            // Directories for uploaded and resized images
            services.Configure<Settings>(Configuration.GetSection("Settings"));

            services.AddMvc();

            // Add image manager
            services.AddSingleton<IImageManager, ImageManager>();
            services.AddSingleton<ISlideshowHandler, SlideshowHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IImageManager imageManager, ISlideshowHandler slideshowHandler)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes
                    .MapRoute( // Special route to load different versions of images
                        name: "image",
                        template: "image/{version}/{id}",
                        defaults: new { controller = "Image", action = "Load" })
                    .MapRoute( // Default route mapped to controller and action
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
            });

            imageManager.Initialize();
            slideshowHandler.Initialize();
        }
    }
}
