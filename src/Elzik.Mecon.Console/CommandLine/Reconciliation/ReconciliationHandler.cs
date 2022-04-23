using Elzik.Mecon.Console.Configuration;
using Elzik.Mecon.Framework.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{
    public static class ReconciliationHandler
    {
        public static void Handle(ConfigurationManager configurationManager, ReconciliationOptions reconciliationOptions)
        {
            try
            {
                configurationManager.AddCommandLineParser(Environment.GetCommandLineArgs());

                var services = Services.Get(configurationManager);

                var reconciledMedia = services.GetRequiredService<IReconciledMedia>();
                var entries = reconciliationOptions.DirectoryKey != null
                    ? AsyncContext.Run(() => reconciledMedia.GetMediaEntries(reconciliationOptions.DirectoryKey))
                    : AsyncContext.Run(() => reconciledMedia.GetMediaEntries(
                        reconciliationOptions.DirectoryPath, reconciliationOptions.FileExtensions,
                        reconciliationOptions.Recurse!.Value, reconciliationOptions.MediaTypes));

                entries = entries.PerformOutputFilters(reconciliationOptions);

                System.Console.WriteLine(
                    string.Join(Environment.NewLine,
                        entries.Select(entry => entry.FilesystemEntry.FileSystemPath)));
            }
            catch (Exception e)
            {
                Environment.ExitCode = 1;
                System.Console.Error.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
