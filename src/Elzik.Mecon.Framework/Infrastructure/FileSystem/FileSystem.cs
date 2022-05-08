using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using Elzik.Mecon.Framework.Infrastructure.FileSystem.Options;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public class FileSystem : IFileSystem
    {
        private readonly IDirectory _directory;
        private readonly System.IO.Abstractions.IFileSystem _fileSystem;
        private readonly FileSystemOptions _fileSystemOptions;

        public FileSystem(IDirectory directory, System.IO.Abstractions.IFileSystem fileSystem,
            IOptions<FileSystemOptions> fileSystemOptions)
        {
            _directory = directory ?? throw new ArgumentNullException(nameof(directory));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            if (fileSystemOptions == null)
            {
                throw new ArgumentNullException(nameof(fileSystemOptions));
            }
            _fileSystemOptions = fileSystemOptions.Value ??
                                 throw new InvalidOperationException($"Value of {nameof(fileSystemOptions)} must not be null.");
        }

        public DirectoryDefinition GetDirectoryDefinition(string directoryDefinitionName)
        {
            if (_fileSystemOptions.DirectoryDefinitions == null || !_fileSystemOptions.DirectoryDefinitions.Any())
            {
                throw new InvalidOperationException("No directory definitions are configured; " +
                                                    $"{directoryDefinitionName} is not found.");
            }

            if (!_fileSystemOptions.DirectoryDefinitions.ContainsKey(directoryDefinitionName))
            {
                throw new InvalidOperationException(
                    $"Directory definition with name of {directoryDefinitionName} is not found.");
            }

            return _fileSystemOptions.DirectoryDefinitions[directoryDefinitionName];
        }

        public IEnumerable<IFileInfo> GetMediaFileInfos(DirectoryDefinition directoryDefinition)
        {
            var files = _directory
                .EnumerateFiles(directoryDefinition.DirectoryPath, "*.*", new EnumerationOptions()
                {
                    RecurseSubdirectories = directoryDefinition.Recurse
                });

            files = FilterFileExtensions(files, directoryDefinition.SupportedFileExtensions);

            files = FilterRegexPattern(files, directoryDefinition.DirectoryFilterRegexPattern);

            var fileInfos = files.Select(filePath =>
                _fileSystem.FileInfo.FromFileName(filePath));

            return fileInfos;
        }

        private static IEnumerable<string> FilterRegexPattern(IEnumerable<string> files, string directoryFilterRegexPattern)
        {
            if (string.IsNullOrWhiteSpace(directoryFilterRegexPattern)) return files;

            var regex = new Regex(directoryFilterRegexPattern, RegexOptions.Compiled);
            files = files.Where(s => regex.IsMatch(s));

            return files;
        }

        private static IEnumerable<string> FilterFileExtensions(IEnumerable<string> files, IEnumerable<string> fileExtensions)
        {
            var extensions = fileExtensions == null
            ? Array.Empty<string>()
            : fileExtensions.Select(extension => $".{extension}").ToArray();

            if (extensions.Any())
            {
                files = files.Where(file => extensions.Any(fileExtension =>
                    file.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase)));
            }

            return files;
        }
    }
}
