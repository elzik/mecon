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

            ValidatePlexConfiguration(plexOptions);
        }

        public async Task<IEnumerable<MediaEntry>> GetMediaEntries(string directoryDefinitionName)
        {
            var directoryDefinition = _fileSystem.GetDirectoryDefinition(directoryDefinitionName);

            var mediaEntries = 
                await GetMediaEntries(directoryDefinition);

            return mediaEntries;
        }

        public async Task<IEnumerable<MediaEntry>> GetMediaEntries(DirectoryDefinition directoryDefinition)
        {
            var mediaFileInfos = _fileSystem.GetMediaFileInfos(directoryDefinition);

            var plexItems = await _plexEntries.GetPlexEntries(directoryDefinition.MediaTypes);

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

                return mediaEntry;
            });

            return mediaEntries;
        }

        private void ValidatePlexConfiguration(IOptions<PlexWithCachingOptions> plexOptions)
        {
            if (string.IsNullOrWhiteSpace(plexOptions.Value.BaseUrl))
            {
                throw new InvalidOperationException("No base URL has been supplied for Plex.");
            }

            if (string.IsNullOrWhiteSpace(plexOptions.Value.AuthToken))
            {
                throw new InvalidOperationException("No auth token has been supplied for Plex.");
            }

            _logger.LogInformation("Plex reconciliation is enabled against {BaseUrl} with {CacheScheme}.",
                    plexOptions.Value.BaseUrl,
                    plexOptions.Value.CacheExpiry.HasValue
                        ? $"a cache expiration of {plexOptions.Value.CacheExpiry} seconds"
                        : "no caching enabled");
        }
    }
}