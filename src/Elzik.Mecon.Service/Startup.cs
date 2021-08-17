using System;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Infrastructure;
using Elzik.Mecon.Framework.Infrastructure.FileSystem;
using Elzik.Mecon.Framework.Infrastructure.FileSystem.Options;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Refit;

namespace Elzik.Mecon.Service
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Elzik.Recon.Service", Version = "v1" });
            });

            services.AddMemoryCache();

            services.AddTransient<IReconciledMedia, MediaReconciler>();

            services.AddTransient<IFileSystem, FileSystem>();
            services.Configure<FileSystemOptions>(Configuration.GetSection("FileSystem"));

            services.Configure<PlexOptionsWithCaching>(Configuration.GetSection("Plex"));
            services.Configure<PlexOptions>(Configuration.GetSection("Plex"));
            services.AddTransient<IPlexEntries, PlexEntriesWithCaching>();
            services.AddTransient<PlexHeaderHandler>();
            services.AddRefitClient<IPlexLibraryClient>(new RefitSettings
                {
                    ContentSerializer = new XmlContentSerializer()
                })
                .ConfigureHttpClient(c =>
                {
                    if (Configuration["Plex:BaseUrl"] != null)
                    {
                        c.BaseAddress = new Uri(Configuration["Plex:BaseUrl"]);
                    }
                })
                .AddHttpMessageHandler<PlexHeaderHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elzik.Recon.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
