using System;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Infrastructure.FileSystem;
using Elzik.Mecon.Framework.Infrastructure.Plex;

namespace Elzik.Mecon.Framework.Application
{
    public class MediaReconciler : IReconciledMedia
    {
        private readonly ILogger<MediaReconciler> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPlexEntries _plexEntries;
        private readonly bool _enablePlex;
     
        public MediaReconciler(ILogger<MediaReconciler> logger, IFileSystem fileSystem, IPlexEntries plexEntries, 
            IOptions<PlexWithCachingOptions> plexOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _plexEntries = plexEntries ?? throw new ArgumentNullException(nameof(plexEntries));
            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }

            _enablePlex = !string.IsNullOrWhiteSpace(plexOptions.Value.AuthToken) &&
                          !string.IsNullOrWhiteSpace(plexOptions.Value.BaseUrl);
            LogPlexConfiguration(plexOptions);
        }

        public async Task<IEnumerable<MediaEntry>> GetMediaEntries(string folderDefinitionName)
        {
            var folderDefinition = _fileSystem.GetFolderDefinition(folderDefinitionName);

            var mediaEntries = 
                await GetMediaEntries(folderDefinition.FolderPath, folderDefinition.SupportedFileExtensions);

            return mediaEntries;
        }

        public async Task<IEnumerable<MediaEntry>> GetMediaEntries(string folderPath, IEnumerable<string> supportedFileExtensions)
        {
            var mediaFileInfos = _fileSystem
                .GetMediaFileInfos(folderPath, supportedFileExtensions);

            var plexItems = new List<PlexEntry>();
            if (_enablePlex)
            {
                plexItems.AddRange(await _plexEntries.GetPlexEntries());
            }

            var mediaEntries = mediaFileInfos.Select(fileInfo =>
            {
                var mediaEntry = new MediaEntry(fileInfo.FullName)
                {
                    FilesystemEntry =
                    {
                        Key = new EntryKey(fileInfo.Name, fileInfo.Length),
                        Title = fileInfo.Name
                    }
                };

                if (_enablePlex)
                {
                    var plexEntries = plexItems
                        .Where(m => m.Key == mediaEntry.FilesystemEntry.Key)
                        .ToList();

                    foreach (var plexEntry in plexEntries)
                    {
                        mediaEntry.ReconciledEntries.Add(plexEntry);
                    }

                    if (plexEntries.Any())
                    {
                        mediaEntry.ThumbnailUrl = plexEntries.First().ThumbnailUrl;
                    }
                }

                return mediaEntry;
            });
            return mediaEntries;
        }

        private void LogPlexConfiguration(IOptions<PlexWithCachingOptions> plexOptions)
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
                _logger.LogInformation("Plex reconciliation is not configured; a BaseUrl and AuthToken must be supplied to enable it.");
            }
        }
    }
}