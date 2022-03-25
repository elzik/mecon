using System;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Infrastructure.FileSystem.Options;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO.Abstractions;
using System.Reflection;
using Plex.Api.Factories;
using Plex.Library.Factories;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;
using FileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.FileSystem;
using IFileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.IFileSystem;

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
            services.AddTransient<IDirectory, DirectoryWrapper>();
            services.AddTransient<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();

            services.Configure<FileSystemOptions>(Configuration.GetSection("FileSystem"));

            services.Configure<PlexWithCachingOptions>(Configuration.GetSection("Plex"));
            services.Configure<PlexOptions>(Configuration.GetSection("Plex"));
            services.AddTransient<IPlexEntries, PlexEntriesWithCaching>();

            // Create Client Options
            var apiOptions = new ClientOptions
            {
                Product = "mecon",
                DeviceName = Environment.MachineName,
                ClientId = "mecon",
                Platform = Environment.OSVersion.Platform.ToString(),
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
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
