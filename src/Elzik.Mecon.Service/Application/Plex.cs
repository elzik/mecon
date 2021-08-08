using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elzik.Mecon.Service.Domain;
using Elzik.Mecon.Service.Infrastructure.ApiClients.Plex;

namespace Elzik.Mecon.Service.Application
{
    public class Plex : IPlex
    {
        private readonly IPlexLibraryClient _plexLibraryClient;

        public Plex(IPlexLibraryClient plexLibraryClient)
        {
            _plexLibraryClient = plexLibraryClient;
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
                            Key = part.File[6..].Replace(@"\", "/"),
                            Title = video.Title,
                            Thumb = video.Thumb
                        };

                        plexEntries.Add(plexEntry);
                    }
                }
            }

            return plexEntries;
        }
    }
}