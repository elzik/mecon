using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Elzik.Mecon.Service.Domain;
using Microsoft.Extensions.Logging;

namespace Elzik.Mecon.Service.Application
{
    public class FileSystemMedia : IFileSystemMedia
    {
        private readonly ILogger<FileSystemMedia> _logger;

        public FileSystemMedia(ILogger<FileSystemMedia> logger)
        {
            _logger = logger;
        }

        public IEnumerable<MediaEntry> GetMediaEntries()
        {
            const string mediaPath = @"\\TOWER\Video\Films";
            const long maximumSmallFileSize = 536870911;
            var matroskaFilePaths = 
                Directory.EnumerateFiles(mediaPath, "*.mkv", SearchOption.AllDirectories);
            var matroskaFileInfos = 
                matroskaFilePaths.Select(matroskaFilePath => new FileInfo(matroskaFilePath)).ToList();
            var largeMatroskaFilInfos = 
                matroskaFileInfos.Where(mfi => mfi.Length > maximumSmallFileSize);

            var smallMatroskaFilInfos = 
                matroskaFileInfos.Where(mfi => mfi.Length <= maximumSmallFileSize).ToList();
            LogFileInfos(smallMatroskaFilInfos);

            var largeMatroskaEntries = largeMatroskaFilInfos.Select(mfi => new MediaEntry()
            {
                FileSystemPath = mfi.FullName
            });

            return largeMatroskaEntries;
        }

        private void LogFileInfos(IList<FileInfo> fileInfos)
        {
            var smallFileTestListBuilder = new StringBuilder();
            foreach (var smallMatroskaFilInfo in fileInfos)
            {
                smallFileTestListBuilder.Append("\n\t");
                smallFileTestListBuilder.Append(smallMatroskaFilInfo.Length / 1024 / 1024);
                smallFileTestListBuilder.Append(" - ");
                smallFileTestListBuilder.Append(smallMatroskaFilInfo.FullName);
            }

            _logger.LogInformation($"{fileInfos.Count} small files ignored:{smallFileTestListBuilder}");
        }
    }
}