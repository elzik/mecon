using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elzik.Mecon.Framework.Domain.Plex
{
    public interface IPlexEntries
    {
        Task<IEnumerable<PlexEntry>> GetPlexEntries(IEnumerable<MediaType> mediaTypesFilter);
    }
}