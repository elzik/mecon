using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
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

        public FolderDefinition GetFolderDefinition(string folderDefinitionName)
        {
            if (_fileSystemOptions.FolderDefinitions == null || !_fileSystemOptions.FolderDefinitions.Any())
            {
                throw new InvalidOperationException("No folder definitions are configured; " +
                                                    $"{folderDefinitionName} is not found.");
            }

            var folderDefinition = _fileSystemOptions.FolderDefinitions
                .SingleOrDefault(option => option.Name == folderDefinitionName);

            if (folderDefinition == null)
            {
                throw new InvalidOperationException(
                    $"Folder definition with name of {folderDefinitionName} is not found.");
            }

            return folderDefinition;
        }

        public IEnumerable<IFileInfo> GetMediaFileInfos(string folderPath, IEnumerable<string> supportedFileExtensions)
        {
            var files = _directory
                .EnumerateFiles(folderPath, "*.*", new EnumerationOptions()
                {
                    RecurseSubdirectories = true
                });

            files = FilterFileExtensions(files, supportedFileExtensions);

            var fileInfos = files.Select(filePath =>
                _fileSystem.FileInfo.FromFileName(filePath));
            return fileInfos;
        }

        private static IEnumerable<string> FilterFileExtensions(IEnumerable<string> files, IEnumerable<string> fileExtensions)
        {
            var extensions = fileExtensions as string[] ?? fileExtensions.ToArray();

            if (extensions.Any())
            {
                files = files.Where(file => extensions.Any(fileExtension =>
                    file.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase)));
            }

            return files;
        }
    }
}
