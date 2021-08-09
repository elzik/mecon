using System;
using Elzik.Mecon.Service.Application;
using Elzik.Mecon.Service.Infrastructure;
using Elzik.Mecon.Service.Infrastructure.Plex;
using Elzik.Mecon.Service.Infrastructure.Plex.ApiClients;
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

            services.AddTransient<IReconciledMedia, ReconciledMedia>();

            services.AddTransient<IFileSystem, FileSystem>();

            services.Configure<PlexOptions>(Configuration.GetSection("Plex"));
            services.AddTransient<IPlex, PlexItems>();
            services.AddTransient<PlexHeaderHandler>();
            services.AddRefitClient<IPlexLibraryClient>(new RefitSettings
                    {
                        ContentSerializer = new XmlContentSerializer()
                    })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["Plex:BaseUrl"]))
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
