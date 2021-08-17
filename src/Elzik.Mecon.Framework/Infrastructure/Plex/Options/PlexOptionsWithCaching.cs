using Elzik.Mecon.Framework.Infrastructure.Plex.Options;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public class PlexOptionsWithCaching : PlexOptions
    {
        public int? CacheExpiry { get; set; }
    }
}
