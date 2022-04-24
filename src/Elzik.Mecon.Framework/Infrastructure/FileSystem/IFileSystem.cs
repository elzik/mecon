using System.Collections.Generic;
using System.IO.Abstractions;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        DirectoryDefinition GetDirectoryDefinition(string directoryDefinitionName);

        IEnumerable<IFileInfo> GetMediaFileInfos(string directoryPath, IEnumerable<string> supportedFileExtensions, bool recurse);
    }
}
