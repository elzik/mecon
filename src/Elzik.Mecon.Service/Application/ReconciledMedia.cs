using Elzik.Mecon.Service.Domain;
using Elzik.Mecon.Service.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Service.Infrastructure.Plex.ApiClients;

namespace Elzik.Mecon.Service.Application
{
    public class ReconciledMedia : IReconciledMedia
    {
        private readonly ILogger<ReconciledMedia> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPlex _plex;
     
        public ReconciledMedia(ILogger<ReconciledMedia> logger, IFileSystem fileSystem, IPlex plex)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _plex = plex;
        }

        public async Task<IEnumerable<MediaEntry>> GetMediaEntries(string mediaPath)
        {
            var mediaFilePaths = _fileSystem.GetMedia(mediaPath);

            var sw = Stopwatch.StartNew();

            var plexItems = await _plex.GetPlexEntries();

            _logger.LogInformation($"Get plex entries took: {sw.Elapsed}");
            sw.Restart();

            var largeMediaEntries = mediaFilePaths.Select(filePath =>
            {
                var fileInfo = new FileInfo(filePath);

                var mediaEntry = new MediaEntry(filePath)
                {
                    FilesystemEntry =
                    {
                        Key = new EntryKey(fileInfo.Name, fileInfo.Length),
                        Title = fileInfo.Name
                    }
                };

                var plexEntry = plexItems.SingleOrDefault(m => m.Key == mediaEntry.FilesystemEntry.Key);

                if (plexEntry != null)
                {
                    mediaEntry.ReconciledEntries.Add(plexEntry);
                    mediaEntry.ThumbnailUrl = plexEntry.ThumbnailUrl;
                }
                
                return mediaEntry;
            });

            _logger.LogInformation($"Find reconciled plex entries tooK: {sw.Elapsed}");


            return largeMediaEntries;
        }
    }
}