using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Elzik.Mecon.Service.Infrastructure
{
    public class FileSystem : IFileSystem
    {
        public IEnumerable<string> GetMedia(string directoryPath, params string[] fileExtensions)
        {
            return Directory.EnumerateFiles(directoryPath, "*.mkv", SearchOption.AllDirectories);
        }
    }
}
