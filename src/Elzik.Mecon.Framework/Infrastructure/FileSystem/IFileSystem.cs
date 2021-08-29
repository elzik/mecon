using System.Collections.Generic;
using System.IO.Abstractions;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        IEnumerable<IFileInfo> GetMediaFileInfos(string folderDefinitionName);
    }
}
