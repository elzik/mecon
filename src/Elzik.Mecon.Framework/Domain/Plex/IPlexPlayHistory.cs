using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elzik.Mecon.Framework.Domain.Plex;

public interface IPlexPlayHistory
{
    Task<IEnumerable<PlayedEntry>> GetPlayHistory();
}