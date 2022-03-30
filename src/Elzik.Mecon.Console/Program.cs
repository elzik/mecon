using Elzik.Mecon.Console.Configuration;
using Elzik.Mecon.Framework.Application;
using Microsoft.Extensions.DependencyInjection;

var config = Configuration.Get();
var services = Services.Get(config);

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