using Elzik.Mecon.Framework.Application;
using Elzik.Mecon.Framework.Domain.FileSystem;
using Microsoft.Extensions.Configuration;
using Nito.AsyncEx;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{

    public class ReconciliationHandler : IReconciliationHandler
    {
        private readonly IReconciledMedia _reconciledMedia;
        private readonly IFileSystem _fileSystem;
        private readonly IOutputOperations _outputOperations;

        public ReconciliationHandler(IReconciledMedia reconciledMedia, IFileSystem fileSystem, IOutputOperations outputOperations)
        {
            _reconciledMedia = reconciledMedia;
            _fileSystem = fileSystem;
            _outputOperations = outputOperations;
        }

        public void Handle(IConfigurationBuilder configurationBuilder, ReconciliationOptions reconciliationOptions)
        {
            try
            {
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

                var closureScopedEntries = entries;
                entries = AsyncContext.Run(() => _outputOperations.PerformOutputFilters(closureScopedEntries, reconciliationOptions));

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
