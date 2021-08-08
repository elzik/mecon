using Elzik.Mecon.Service.Domain;
using Elzik.Mecon.Service.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            const string plexLibrary = @"Films";
            const long maximumSmallFileSize = 536870911;

            var mediaFilePaths = _fileSystem.GetMedia(mediaPath);
            var largeMediaFilePaths = _fileSystem.GetLargeMediaEntries(mediaFilePaths, maximumSmallFileSize);

            var sw = Stopwatch.StartNew();

            var plexItems = await _plex.GetPlexItems(plexLibrary);

            _logger.LogInformation($"Get plex entries tooK: {sw.Elapsed}");
            sw.Restart();

            var largeMediaEntries = largeMediaFilePaths.Select(filePath =>
            {
                var mediaEntry = new MediaEntry(filePath)
                {
                    FilesystemEntry =
                    {
                        Key = filePath.Replace(mediaPath, string.Empty).Replace(@"\", "/"),
                        Title = new FileInfo(filePath).Name
                    }
                };

                var plexEntry = plexItems.SingleOrDefault(m =>
                    m.Key.Equals(mediaEntry.FilesystemEntry.Key, StringComparison.InvariantCultureIgnoreCase));

                if (plexEntry != null)
                {
                    mediaEntry.ReconciledEntries.Add(plexEntry);
                }
                

                return mediaEntry;
            });

            _logger.LogInformation($"Find reconciled plex entries tooK: {sw.Elapsed}");


            return largeMediaEntries;
        }
    }
}