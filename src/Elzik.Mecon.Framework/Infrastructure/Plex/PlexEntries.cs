using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.Options;
using Flurl;
using Microsoft.Extensions.Options;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexEntries : IPlexEntries
    {
        private readonly IPlexServerClient _plexServerClient;
        private readonly IPlexLibraryClient _plexLibraryClient;
        private readonly PlexOptions _plexOptions;

        public PlexEntries(IPlexServerClient plexServerClient, IPlexLibraryClient plexLibraryClient, IOptions<PlexOptions> plexOptions)
        {
            _plexServerClient = plexServerClient ?? throw new ArgumentNullException(nameof(plexServerClient));
            _plexLibraryClient = plexLibraryClient ?? throw new ArgumentNullException(nameof(plexLibraryClient));

            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }
            _plexOptions = plexOptions.Value ?? 
                           throw new InvalidOperationException($"Value of {nameof(plexOptions)} must not be null.");
        }

        public virtual async Task<IEnumerable<PlexEntry>> GetPlexEntries()
        {
            var plexEntries = new List<PlexEntry>();
            var libraryContainer =
                await _plexServerClient.GetLibrariesAsync(_plexOptions.AuthToken, _plexOptions.BaseUrl);
            var videoTypes = new[] {"movie"};
            var videoLibraries = libraryContainer.Libraries.Where(library => videoTypes.Contains(library.Type));

            foreach (var library in videoLibraries)
            {
                var entries = await GetPlexEntries(library);

                plexEntries.AddRange(entries);
            }

            return plexEntries;
        }

        private async Task<IEnumerable<PlexEntry>> GetPlexEntries(Library library)
        {
            var videoCount = await _plexLibraryClient.GetLibrarySize(_plexOptions.AuthToken, _plexOptions.BaseUrl, library.Key);
            var mediaContainer = await _plexLibraryClient.LibrarySearch(_plexOptions.AuthToken, _plexOptions.BaseUrl,
                string.Empty, library.Key, String.Empty, SearchType.Movie, count: videoCount);
            var mediaItems = mediaContainer.Media.Where(video => video.Type != "collection");

            var plexEntries = 
                from video in mediaItems
                from medium in video.Media
                from part in medium.Part
                select new PlexEntry()
                {
                    Key = new EntryKey(Path.GetFileName(part.File), part.Size),
                    Title = video.Title,
                    ThumbnailUrl = _plexOptions.BaseUrl.AppendPathSegment(video.Thumb)
                        .SetQueryParam("X-Plex-Token", _plexOptions.AuthToken)
                };

            return plexEntries;
        }
    }
}