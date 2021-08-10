using Elzik.Mecon.Service.Domain;
using Elzik.Mecon.Service.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Service.Infrastructure.Plex;
using Elzik.Mecon.Service.Infrastructure.Plex.ApiClients;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Service.Application
{
    public class MediaReconciler : IReconciledMedia
    {
        private readonly ILogger<MediaReconciler> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPlex _plex;
        private readonly bool _enablePlex;
     
        public MediaReconciler(ILogger<MediaReconciler> logger, IFileSystem fileSystem, IPlex plex, IOptions<PlexOptionsWithCaching> plexOptions)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _plex = plex;
            _enablePlex = plexOptions.Value is {AuthToken: { }, BaseUrl: { }};
            LogPlexConfiguration(plexOptions);
        }

        public async Task<IEnumerable<MediaEntry>> GetMediaEntries(string mediaPath)
        {
            var mediaFilePaths = _fileSystem.GetMedia(mediaPath);

            var plexItems = new List<PlexEntry>();
            if (_enablePlex)
            {
                plexItems.AddRange(await _plex.GetPlexEntries());
            }

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

                if (_enablePlex)
                {
                    var plexEntry = plexItems.SingleOrDefault(m => m.Key == mediaEntry.FilesystemEntry.Key);

                    if (plexEntry != null)
                    {
                        mediaEntry.ReconciledEntries.Add(plexEntry);
                        mediaEntry.ThumbnailUrl = plexEntry.ThumbnailUrl;
                    }
                }

                return mediaEntry;
            });

            return largeMediaEntries;
        }

        private void LogPlexConfiguration(IOptions<PlexOptionsWithCaching> plexOptions)
        {
            if (_enablePlex)
            {
                _logger.LogInformation("Plex reconciliation is enabled against {BaseUrl} with {CacheScheme}.",
                    plexOptions.Value.BaseUrl,
                    plexOptions.Value.CacheExpiry.HasValue
                        ? $"a cache expiration of {plexOptions.Value.CacheExpiry} seconds"
                        : "no caching enabled");
            }
            else
            {
                _logger.LogInformation("Plex reconciliation is not configured; a BaseUrl and AuthToken must be supplied.");
            }
        }
    }
}