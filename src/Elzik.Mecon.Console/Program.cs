using CommandLine;
using Elzik.Mecon.Console;
using Elzik.Mecon.Console.CommandLine;
using Elzik.Mecon.Console.Configuration;
using Elzik.Mecon.Framework.Application;
using Microsoft.Extensions.DependencyInjection;

await Parser.Default.ParseArguments<MeconOptions>(args)
    .WithParsedAsync(async options => 
    {
        var config = Configuration.Get();
        var services = Services.Get(config);

        var reconciledMedia = services.GetRequiredService<IReconciledMedia>();

        try
        {
            var entries = options.DirectoryName != null ? 
                await reconciledMedia.GetMediaEntries(options.DirectoryName) : 
                await reconciledMedia.GetMediaEntries(options.DirectoryPath, options.FileExtensions, options.Recurse!.Value);

            entries = entries.PerformOutputFilters(options);

            Console.WriteLine(
                string.Join(Environment.NewLine, 
                    entries.Select(entry => entry.FilesystemEntry.FileSystemPath)));
        }
        catch (Exception e)
        {
            Environment.ExitCode = 1;
            Console.Error.WriteLine(e.Message);
        }
    });