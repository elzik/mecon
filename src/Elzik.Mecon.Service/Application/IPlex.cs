using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Service.Domain;

namespace Elzik.Mecon.Service.Application
{
    public interface IPlex
    {
        Task<IEnumerable<PlexEntry>> GetPlexItems(string plexLibraryKey);
    }
}