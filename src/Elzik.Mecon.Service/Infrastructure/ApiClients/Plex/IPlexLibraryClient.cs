using System.Threading.Tasks;
using Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models;
using Refit;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex
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
