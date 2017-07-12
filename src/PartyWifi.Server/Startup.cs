using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PartyWifi.Server.Components;
using PartyWifi.Server.Model;
using Swashbuckle.AspNetCore.Swagger;

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

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            // Directories for uploaded and resized images
            services.Configure<Settings>(Configuration.GetSection("Settings"));

            services.AddMvc();

            // add swagger to the services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "PartyWifi API", Version = "v1" });
            });

            // Add image manager
            services.AddSingleton<IImageManager, ImageManager>();
            services.AddSingleton<ISlideshowHandler, SlideshowHandler>();
            services.AddSingleton<IUnitOfWorkFactory, PartyWifiUnitOfWorkFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IImageManager imageManager, ISlideshowHandler slideshowHandler)
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

            // Use swagger and ui middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PartyWifi v1");
            });

            app.UseMvc(routes =>
            {
                routes
                    .MapRoute( // Default route mapped to controller and action
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
            });

            imageManager.Initialize();
            slideshowHandler.Initialize();
        }
    }
}
