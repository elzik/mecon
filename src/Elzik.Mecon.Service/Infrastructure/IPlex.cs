using Elzik.Mecon.Service.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elzik.Mecon.Service.Infrastructure
{
    public interface IPlex
    {
        Task<IEnumerable<PlexEntry>> GetPlexItems(string plexLibraryKey);
    }
}