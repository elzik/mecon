using System.Threading.Tasks;
using Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models;
using Refit;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients
{
    [Headers("X-Plex-Client-Identifier", "X-Plex-Token")]
    public interface IPlexLibraryClient
    {
        [Get("/library/sections")]
        Task<LibraryContainer> GetLibraries();

        [Get("/library/sections/{libraryKey}/all")]
        Task<MediaContainer> GetMedia(string libraryKey);
    }
}
