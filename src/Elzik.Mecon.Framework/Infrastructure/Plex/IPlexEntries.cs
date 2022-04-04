using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public interface IPlexEntries
    {
        Task<IEnumerable<PlexEntry>> GetPlexEntries(IEnumerable<MediaType> mediaTypesFilter);
    }
}