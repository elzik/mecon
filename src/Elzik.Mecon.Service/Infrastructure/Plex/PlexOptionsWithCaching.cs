using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elzik.Mecon.Service.Infrastructure.Plex
{
    public class PlexOptionsWithCaching : PlexOptions
    {
        public int? CacheExpiry { get; set; }
    }
}
