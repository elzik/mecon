using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Infrastructure.FileSystem;
using Microsoft.Extensions.Configuration;
using Nito.AsyncEx;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{

    public class ReconciliationHandler : IReconciliationHandler
    {
        private readonly IReconciledMedia _reconciledMedia;
        private readonly IFileSystem _fileSystem;

        public ReconciliationHandler(IReconciledMedia reconciledMedia, IFileSystem fileSystem)
        {
            _reconciledMedia = reconciledMedia;
            _fileSystem = fileSystem;
        }

        public void Handle(ConfigurationManager configurationManager, ReconciliationOptions reconciliationOptions)
        {
            try
            {
                configurationManager.AddCommandLineParser(Environment.GetCommandLineArgs());

                var directoryDefinition = reconciliationOptions.DirectoryKey == null
                    ? new DirectoryDefinition()
                    {
                        SupportedFileExtensions =
                            (reconciliationOptions.FileExtensions ?? Array.Empty<string>()).ToArray(),
                        MediaTypes = reconciliationOptions.MediaTypes,
                        Recurse = reconciliationOptions.Recurse ?? false,
                        DirectoryFilterRegexPattern = reconciliationOptions.MatchRegex,
                        DirectoryPath = reconciliationOptions.DirectoryPath
                    }
                    : _fileSystem.GetDirectoryDefinition(reconciliationOptions.DirectoryKey);

                var entries = AsyncContext.Run(() => _reconciledMedia.GetMediaEntries(directoryDefinition));

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
