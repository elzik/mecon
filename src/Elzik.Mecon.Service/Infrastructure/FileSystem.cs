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

        public IEnumerable<string> GetLargeMediaEntries(IEnumerable<string> mediaFilePaths, long maximumSmallFileSize)
        {
            var mediaFileInfos =
                mediaFilePaths.Select(matroskaFilePath => new FileInfo(matroskaFilePath)).ToList();
            var largeMediaFilePaths =
                mediaFileInfos.Where(mfi => mfi.Length > maximumSmallFileSize).Select(info => info.FullName);

            return largeMediaFilePaths;
        }
    }
}
