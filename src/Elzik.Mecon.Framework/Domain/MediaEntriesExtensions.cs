using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elzik.Mecon.Framework.Domain
{
    public static class MediaEntriesExtensions
    {
        public static IEnumerable<MediaEntry> WhereNotInPlex(this IEnumerable<MediaEntry> entries)
        {
            return entries.Where(entry => !entry.ReconciledEntries.Any(entry1 => entry1 is PlexEntry));
        }
    }
}
