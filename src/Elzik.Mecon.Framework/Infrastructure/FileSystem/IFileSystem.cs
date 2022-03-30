using System.Collections.Generic;
using System.IO.Abstractions;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        FolderDefinition GetFolderDefinition(string folderDefinitionName);

        IEnumerable<IFileInfo> GetMediaFileInfos(string folderPath, string[] supportedFileExtensions);
    }
}
