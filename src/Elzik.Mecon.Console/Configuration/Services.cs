using System.IO.Abstractions;
using System.Reflection;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Infrastructure.FileSystem.Options;
using Elzik.Mecon.Framework.Infrastructure.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plex.Api.Factories;
using Plex.Library.Factories;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;
using FileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.FileSystem;
using IFileSystem = Elzik.Mecon.Framework.Infrastructure.FileSystem.IFileSystem;

namespace Elzik.Mecon.Console.Configuration
{
    internal static class Services
    {
        public static ServiceProvider Get(ConfigurationManager configurationManager)
        {
            var fullAssemblyName = Assembly.GetExecutingAssembly().GetName();

            var version = fullAssemblyName.Version;
            if (version == null) throw new InvalidOperationException("It was not possible to get the assembly version.");

            var apiOptions = new ClientOptions
            {
                Product = fullAssemblyName.Name,
                DeviceName = Environment.MachineName,
                ClientId = fullAssemblyName.Name,
                Platform = Environment.OSVersion.Platform.ToString(),
                Version = version.ToString()
            };

            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(configurationManager.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .AddSingleton<IReconciliationHandler, ReconciliationHandler>()
                .AddSingleton<IReconciledMedia, MediaReconciler>()
                .AddTransient<IFileSystem, FileSystem>()
                .AddTransient<IDirectory, DirectoryWrapper>()
                .AddTransient<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>()
                .Configure<FileSystemOptions>(configurationManager.GetSection("FileSystem"))
                .Configure<PlexWithCachingOptions>(configurationManager.GetSection("Plex"))
                .Configure<PlexOptions>(configurationManager.GetSection("Plex"))
                .AddTransient<IPlexEntries, PlexEntries>()
                .AddSingleton(apiOptions)
                .AddTransient<IPlexServerClient, PlexServerClient>()
                .AddTransient<IPlexAccountClient, PlexAccountClient>()
                .AddTransient<IPlexLibraryClient, PlexLibraryClient>()
                .AddTransient<IApiService, ApiService>()
                .AddTransient<IPlexFactory, PlexFactory>()
                .AddTransient<IPlexRequestsHttpClient, PlexRequestsHttpClient>()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
