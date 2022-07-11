using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.Plex;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Flurl;
using Microsoft.Extensions.Options;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Plex.ServerApi.PlexModels.Media;
using MediaType = Elzik.Mecon.Framework.Domain.MediaType;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexEntries : IPlexEntries
    {
        private readonly PlexOptions _plexOptions;
        private readonly IPlexServerClient _plexServerClient;
        private readonly IPlexLibraryClient _plexLibraryClient;
        private readonly IPlexPlayHistory _plexPlayHistory;
        private IEnumerable<PlayedEntry> _playHistory;

        public PlexEntries(IPlexServerClient plexServerClient, IPlexLibraryClient plexLibraryClient, IOptions<PlexOptions> plexOptions, IPlexPlayHistory plexPlayHistory)
        {
            _plexServerClient = plexServerClient ?? throw new ArgumentNullException(nameof(plexServerClient));
            _plexLibraryClient = plexLibraryClient ?? throw new ArgumentNullException(nameof(plexLibraryClient));
            _plexPlayHistory = plexPlayHistory ?? throw new ArgumentNullException(nameof(plexPlayHistory));

            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }
            _plexOptions = plexOptions.Value ?? 
                           throw new InvalidOperationException($"Value of {nameof(plexOptions)} must not be null.");
        }

        public virtual async Task<IEnumerable<PlexEntry>> GetPlexEntries(IEnumerable<MediaType> mediaTypesFilter)
        {
            _playHistory = await _plexPlayHistory.GetPlayHistory();

            var plexEntries = new List<PlexEntry>();
            var libraryContainer =
                await _plexServerClient.GetLibrariesAsync(_plexOptions.AuthToken, _plexOptions.BaseUrl);
            var libraryTypes = mediaTypesFilter.ToPlexLibraryTypes();
            var videoLibraries = libraryContainer.Libraries.Where(library => libraryTypes.Contains(library.Type));

            foreach (var library in videoLibraries)
            {
                var entries = await GetPlexEntries(library);

                plexEntries.AddRange(entries);
            }

            return plexEntries;
        }

        private async Task<IEnumerable<PlexEntry>> GetPlexEntries(Library library)
        {
            var mediaItems = await GetLibraryItems(library);

            var plexEntries = 
                from video in mediaItems
                from medium in video.Media
                from part in medium.Part
                select new PlexEntry()
                {
                    Key = new EntryKey(Path.GetFileName(part.File), part.Size),
                    Title = video.Title,
                    ThumbnailUrl = _plexOptions.BaseUrl.AppendPathSegment(video.Thumb)
                        .SetQueryParam("X-Plex-Token", _plexOptions.AuthToken),
                    WatchedByAccounts = GetWatchedByUsers(video)
                };

            return plexEntries;
        }

        private IEnumerable<int> GetWatchedByUsers(Metadata part)
        {
            var watchedByAccountIds = _playHistory
                .Where(entry => entry.LibraryKey == part.Key)
                .DistinctBy(entry => entry.AccountId)
                .Select(entry => entry.AccountId);

            return watchedByAccountIds;
        }

        private async Task<List<Metadata>> GetLibraryItems(Library library)
        {
            var searchType = library.Type switch
            {
                "movie" => SearchType.Movie,
                "show" => SearchType.Episode,
                _ => throw new InvalidOperationException($"Unsupported library type {library.Type}.")
            };

            var mediaItems = new List<Metadata>();
            var consumptionCount = 0;
            MediaContainer mediaContainer;
            do
            {
                mediaContainer = await _plexLibraryClient.LibrarySearch(_plexOptions.AuthToken, _plexOptions.BaseUrl,
                    string.Empty, library.Key, string.Empty, searchType, count: _plexOptions.ItemsPerCall,
                    start: consumptionCount);

                consumptionCount += mediaContainer.Size;

                if (mediaContainer.Media != null)
                {
                    mediaItems.AddRange(mediaContainer.Media.Where(video => video.Type != "collection"));
                }
            } while (consumptionCount < mediaContainer.TotalSize);

            return mediaItems;
        }
    }
}