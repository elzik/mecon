using CommandLine;
using Elzik.Mecon.Console.CommandLine;
using Elzik.Mecon.Console.Configuration;
using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Domain;
using Microsoft.Extensions.DependencyInjection;

await Parser.Default.ParseArguments<MeconOptions>(args)
    .WithParsedAsync(async options => 
    {
        var config = Configuration.Get();
        var services = Services.Get(config);

        var reconciledMedia = services.GetRequiredService<IReconciledMedia>();

        try
        {
            IEnumerable<MediaEntry> entries = options.DirectoryName != null ? 
                await reconciledMedia.GetMediaEntries(options.DirectoryName) : 
                await reconciledMedia.GetMediaEntries(options.DirectoryPath, options.FileExtensions);

            if (options.MissingFromLibrary)
            {
                entries = entries.WhereNotInPlex();
            }

            foreach (var entry in entries)
            {
                Console.WriteLine(entry.FilesystemEntry.FileSystemPath);
            }
        }
        catch (Exception e)
        {
            Environment.ExitCode = 1;
            Console.Error.WriteLine(e.Message);
        }
    });