using System.Collections.Generic;
using System.IO;

namespace Elzik.Mecon.Framework.Infrastructure
{
    public class FileSystem : IFileSystem
    {
        public IEnumerable<string> GetMedia(string directoryPath, params string[] fileExtensions)
        {
            return Directory.EnumerateFiles(directoryPath, "*.mkv", SearchOption.AllDirectories);
        }
    }
}
