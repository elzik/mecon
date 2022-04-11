using System.Text;
using System.Text.Json;
using CommandLine;
using Elzik.Mecon.Console;
using Elzik.Mecon.Console.CommandLine;
using Elzik.Mecon.Console.Configuration;
using Elzik.Mecon.Framework.Application;
using Microsoft.Extensions.DependencyInjection;

var config = Configuration.Get();

Parser.Default.ParseArguments<MeconOptions, ConfigOptions>(args)
    .WithParsed<MeconOptions>(options => 
    {
        try
        {
            config.AddCommandLineParser(Environment.GetCommandLineArgs());

            var services = Services.Get(config);

            var reconciledMedia = services.GetRequiredService<IReconciledMedia>();
            var entries = options.DirectoryName != null ? 
                reconciledMedia.GetMediaEntries(options.DirectoryName).Result : 
                reconciledMedia.GetMediaEntries(options.DirectoryPath, options.FileExtensions, options.Recurse!.Value, options.MediaTypes).Result;

            entries = entries.PerformOutputFilters(options);

            Console.WriteLine(
                string.Join(Environment.NewLine, 
                    entries.Select(entry => entry.FilesystemEntry.FileSystemPath)));
        }
        catch (Exception e)
        {
            Environment.ExitCode = 1;
            Console.Error.WriteLine($"Error: {e.Message}");
        }
    }).WithParsed<ConfigOptions>(_ => Console.WriteLine(config.ToJsonString()));

