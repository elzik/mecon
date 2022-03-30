using System.IO.Abstractions;
using System.Reflection;
using Elzik.Mecon.Console.CommandLine;
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

var config = new ConfigurationManager();
config.AddJsonFile("appsettings.json", false);
config.AddJsonFile("appsettings.Development.json", true);
config.AddCommandLineParser(args);

var version = Assembly.GetExecutingAssembly().GetName().Version;
if (version == null) throw new InvalidOperationException("It was not possible to get the assembly version.");
var apiOptions = new ClientOptions
{
    Product = "mecon",
    DeviceName = Environment.MachineName,
    ClientId = "mecon",
    Platform = Environment.OSVersion.Platform.ToString(),
    Version = version.ToString()
};

var services = new ServiceCollection()
    .AddLogging(loggingBuilder => loggingBuilder.AddSimpleConsole(options => { options.SingleLine = true; }))
    .AddSingleton<IReconciledMedia, MediaReconciler>()
    .AddTransient<IFileSystem, FileSystem>()
    .AddTransient<IDirectory, DirectoryWrapper>()
    .AddTransient<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>()
    .Configure<FileSystemOptions>(config.GetSection("FileSystem"))
    .Configure<PlexWithCachingOptions>(config.GetSection("Plex"))
    .Configure<PlexOptions>(config.GetSection("Plex"))
    .AddTransient<IPlexEntries, PlexEntries>()
    .AddSingleton(apiOptions)
    .AddTransient<IPlexServerClient, PlexServerClient>()
    .AddTransient<IPlexAccountClient, PlexAccountClient>()
    .AddTransient<IPlexLibraryClient, PlexLibraryClient>()
    .AddTransient<IApiService, ApiService>()
    .AddTransient<IPlexFactory, PlexFactory>()
    .AddTransient<IPlexRequestsHttpClient, PlexRequestsHttpClient>()
    .BuildServiceProvider();

var reconciledMedia = services.GetService<IReconciledMedia>();
if (reconciledMedia == null) throw new InvalidOperationException("Failed to instantiate ReconciledMedia.");

try
{
    var entries = await reconciledMedia.GetMediaEntries("Films");

    var badEntries = entries.Where(entry => entry.ReconciledEntries.Count == 0);

    foreach (var badEntry in badEntries)
    {
        Console.WriteLine(badEntry.FilesystemEntry.FileSystemPath);
    }
}
catch (Exception e)
{
    Environment.ExitCode = 1;
    Console.Error.WriteLine(e.Message);
    
}