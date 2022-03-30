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

        var reconciledMedia = services.GetService<IReconciledMedia>();
        if (reconciledMedia == null) throw new InvalidOperationException("Failed to instantiate ReconciledMedia.");

        try
        {
            IEnumerable<MediaEntry> entries = options.DirectoryName != null ? 
                await reconciledMedia.GetMediaEntries(options.DirectoryName) : 
                await reconciledMedia.GetMediaEntries(options.DirectoryPath, options.FileExtensions);

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
    });