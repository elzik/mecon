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
        private readonly FileSystemOptions _fileSystemOptions;

        public FileSystem(IDirectory directory, IOptions<FileSystemOptions> fileSystemOptions)
        {
            _directory = directory ?? throw new ArgumentNullException(nameof(directory));

            if (fileSystemOptions == null)
            {
                throw new ArgumentNullException(nameof(fileSystemOptions));
            }
            _fileSystemOptions = fileSystemOptions.Value ??
                                 throw new InvalidOperationException($"Value of {nameof(fileSystemOptions)} must not be null.");
        }

        public IEnumerable<string> GetMediaFilePaths(string folderDefinitionName)
        {
            var folderDefinition = _fileSystemOptions.FolderDefinitions
                .SingleOrDefault(option => option.Name == folderDefinitionName);
            if (folderDefinition == null)
            {
                throw new InvalidOperationException(
                    $"Folder definition with name of {folderDefinitionName} is not found.");
            }

            var files = _directory
                .EnumerateFiles(folderDefinition.FolderPath, "*.*", new EnumerationOptions()
                {
                    RecurseSubdirectories = true
                });

            files = FilterFileExtensions(files, folderDefinition.SupportedFileExtensions);

            return files;
        }

        private static IEnumerable<string> FilterFileExtensions(IEnumerable<string> files, string[] fileExtensions)
        {
            if (fileExtensions != null && fileExtensions.Any())
            {
                files = files.Where(file => fileExtensions.Any(fileExtension =>
                    file.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase)));
            }

            return files;
        }
    }
}
