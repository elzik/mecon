using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models;
using Microsoft.Extensions.Options;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexEntries : IPlex
    {
        private readonly IPlexLibraryClient _plexLibraryClient;
        private readonly PlexOptions _plexOptions;

        public PlexEntries(IPlexLibraryClient plexLibraryClient, IOptions<PlexOptions> plexOptions)
        {
            ValidateOptions(plexOptions);

            _plexLibraryClient = plexLibraryClient;
            _plexOptions = plexOptions.Value;
        }

        public virtual async Task<IEnumerable<PlexEntry>> GetPlexEntries()
        {
            var plexEntries = new List<PlexEntry>();
            var libraryContainer = await _plexLibraryClient.GetLibraries();

            foreach (var library in libraryContainer.Directory)
            {
                var entries = await GetPlexEntries(library);

                plexEntries.AddRange(entries);
            }

            return plexEntries;
        }

        private async Task<List<PlexEntry>> GetPlexEntries(Library library)
        {
            var entries = new List<PlexEntry>();
            var mediaContainer = await _plexLibraryClient.GetMedia(library.Key);
            var videos = mediaContainer.Video.Where(video => video.Type != "collection");

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

                        entries.Add(plexEntry);
                    }
                }
            }

            return entries;
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
        }
    }
}