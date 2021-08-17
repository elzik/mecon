using System.Collections.Generic;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public interface IFileSystem
    {
        IEnumerable<string> GetMediaFilePaths(string folderDefinitionName);
    }
}
