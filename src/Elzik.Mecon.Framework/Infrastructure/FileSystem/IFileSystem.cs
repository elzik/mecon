using System.Collections.Generic;
using System.IO.Abstractions;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        FolderDefinition GetFolderDefinition(string folderDefinitionKey);

        IEnumerable<IFileInfo> GetMediaFileInfos(string folderPath, IEnumerable<string> supportedFileExtensions, bool recurse);
    }
}
