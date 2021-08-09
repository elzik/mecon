using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Service.Domain;
using Elzik.Mecon.Service.Infrastructure.Plex.ApiClients;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Service.Infrastructure.Plex
{
    public class PlexItems : IPlex
    {
        private readonly IPlexLibraryClient _plexLibraryClient;
        private readonly PlexOptions _plexOptions;

        public PlexItems(IPlexLibraryClient plexLibraryClient, IOptions<PlexOptions> plexOptions)
        {
            ValidateOptions(plexOptions);

            _plexLibraryClient = plexLibraryClient;
            _plexOptions = plexOptions.Value;
        }

        public async Task<IEnumerable<PlexEntry>> GetPlexItems(string plexLibraryKey)
        {
            var libraryContainer = await _plexLibraryClient.GetLibraries();

            var library = libraryContainer.Directory.Find(l => l.Title == plexLibraryKey);
            if (library == null)
            {
                throw new InvalidOperationException($"The Plex Library with key {plexLibraryKey} cannot be found.");
            }

            var mediaContainer = await _plexLibraryClient.GetMedia(library.Key);

            var videos = mediaContainer.Video.Where(video => video.Type != "collection");

            var plexEntries = new List<PlexEntry>();
            foreach (var video in videos)
            {
                foreach (var medium in video.Media)
                {
                    foreach (var part in medium.Parts)
                    {
                        var plexEntry = new PlexEntry()
                        {
                            Key = new EntryKey(
                                Path.GetFileName(part.File), 
                                long.Parse(part.Size)),
                            Title = video.Title,
                            ThumbnailUrl = $"{_plexOptions.BaseUrl}{video.Thumb}?X-Plex-Token={_plexOptions.AuthToken}"
                    };

                        plexEntries.Add(plexEntry);
                    }
                }
            }

            return plexEntries;
        }

        private static void ValidateOptions(IOptions<PlexOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value == null)
            {
                throw new InvalidOperationException($"{nameof(options)} must not be null.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.AuthToken))
            {
                throw new InvalidOperationException($"{nameof(options)} must contain an {nameof(options.Value.AuthToken)}.");
            }

            if (string.IsNullOrWhiteSpace(options.Value.BaseUrl))
            {
                throw new InvalidOperationException($"{nameof(options)} must contain a {nameof(options.Value.BaseUrl)}.");
            }
        }
    }
}