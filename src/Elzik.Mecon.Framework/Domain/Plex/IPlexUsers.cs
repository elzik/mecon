using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elzik.Mecon.Framework.Domain.Plex;

public interface IPlexUsers
{
    Task<IEnumerable<PlexUser>> GetPlexUsers();
}