using Elzik.Mecon.Service.Application;
using Elzik.Mecon.Service.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Plex.Api.Factories;
using Plex.Library.Factories;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;

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

            services.AddTransient<IMedia, Media>();

            services.AddTransient<IFileSystem, FileSystem>();
            services.AddTransient<IPlex, Infrastructure.Plex>();

            var apiOptions = new ClientOptions
            {
                Product = "API_UnitTests",
                DeviceName = "API_UnitTests",
                ClientId = "MyClientId",
                Platform = "Web",
                Version = "v2"
            };
            services.AddSingleton(apiOptions);
            services.AddTransient<IPlexServerClient, PlexServerClient>();
            services.AddTransient<IPlexAccountClient, PlexAccountClient>();
            services.AddTransient<IPlexLibraryClient, PlexLibraryClient>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IPlexFactory, PlexFactory>();
            services.AddTransient<IPlexRequestsHttpClient, PlexRequestsHttpClient>();
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
