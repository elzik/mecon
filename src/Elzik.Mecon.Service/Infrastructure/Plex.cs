using Elzik.Mecon.Service.Domain;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elzik.Mecon.Service.Infrastructure
{
    public class Plex : IPlex
    {
        private const string Token = "KPSTWHM3Y3_XYL1Vfjms";
        private readonly IPlexLibraryClient _plexLibraryClient;
        private readonly IPlexServerClient _plexServerClient;

        public Plex(IPlexServerClient plexServerClient, IPlexLibraryClient plexLibraryClient)
        {
            _plexServerClient = plexServerClient;
            _plexLibraryClient = plexLibraryClient;
        }

        public async Task<IEnumerable<PlexEntry>> GetPlexItems(string plexLibraryKey)
        {
            var libraries = await _plexServerClient.GetLibrariesAsync(Token, "http://192.168.0.12:32400/");

            var library = libraries.Libraries.Find(l => l.Title == plexLibraryKey);
            if (library == null)
            {
                throw new InvalidOperationException($"The Plex Library with key {plexLibraryKey} cannot be found.");
            }

            var libraryCount =
                await _plexLibraryClient.GetLibrarySize(Token, "http://192.168.0.12:32400/", library.Key);

            var libraryContainer = await _plexLibraryClient.LibrarySearch(
                Token, 
                "http://192.168.0.12:32400/",
                null,
                library.Key,
                null, 
                Enum.Parse<SearchType>(library.Type, true), 
                null, 
                0, 
                libraryCount);

            var nonCollectionMedia = libraryContainer.Media.Where(metadata => metadata.Type != "collection");

            var plexEntries = new List<PlexEntry>();
            foreach (var metaData in nonCollectionMedia)
            {
                foreach (var medium in metaData.Media)
                {
                    foreach (var part in medium.Part)
                    {
                        var plexEntry = new PlexEntry()
                        {
                            Key = part.File[6..].Replace(@"\", "/"),
                            Title = metaData.Title,
                            Thumb = metaData.Thumb
                        };

                        plexEntries.Add(plexEntry);
                    }
                }
            }

            return plexEntries;
        }
    }
}