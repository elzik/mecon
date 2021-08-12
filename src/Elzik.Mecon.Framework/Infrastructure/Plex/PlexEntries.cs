using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models;
using Flurl;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexEntries : IPlexEntries
    {
        private readonly IPlexLibraryClient _plexLibraryClient;
        private readonly PlexOptions _plexOptions;

        public PlexEntries(IPlexLibraryClient plexLibraryClient, IOptions<PlexOptions> plexOptions)
        {
            _plexLibraryClient = plexLibraryClient ?? throw new ArgumentNullException(nameof(plexLibraryClient));

            if (plexOptions == null)
            {
                throw new ArgumentNullException(nameof(plexOptions));
            }
            _plexOptions = plexOptions.Value ?? 
                           throw new InvalidOperationException($"{nameof(plexOptions)} must not be null.");
        }

        public virtual async Task<IEnumerable<PlexEntry>> GetPlexEntries()
        {
            var plexEntries = new List<PlexEntry>();
            var libraryContainer = await _plexLibraryClient.GetLibraries();
            var videoTypes = new[] {"movie"};
            var videoLibraries = libraryContainer.Directory.Where(library => videoTypes.Contains(library.Type));

            foreach (var library in videoLibraries)
            {
                var entries = await GetPlexEntries(library);

                plexEntries.AddRange(entries);
            }

            return plexEntries;
        }

        private async Task<IEnumerable<PlexEntry>> GetPlexEntries(Library library)
        {
            var mediaContainer = await _plexLibraryClient.GetMedia(library.Key);
            var videos = mediaContainer.Video.Where(video => video.Type != "collection");

            var plexEntries = from video in videos
                from medium in video.Media
                from part in medium.Parts
                select new PlexEntry()
                {
                    Key = new EntryKey(Path.GetFileName(part.File), long.Parse(part.Size)),
                    Title = video.Title,
                    ThumbnailUrl = _plexOptions.BaseUrl.AppendPathSegment(video.Thumb)
                        .SetQueryParam("X-Plex-Token", _plexOptions.AuthToken)
                };

            return plexEntries;
        }
    }
}