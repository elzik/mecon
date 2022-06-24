using System;
using Elzik.Mecon.Framework.Domain.Plex;
using System.Collections.Generic;
using System.Linq;

namespace Elzik.Mecon.Framework.Domain
{
    public static class MediaEntriesExtensions
    {
        public static IEnumerable<MediaEntry> WhereNotInPlex(this IEnumerable<MediaEntry> entries)
        {
            return entries.Where(entry => !entry.ReconciledEntries.Any(entry1 => entry1 is PlexEntry));
        }

        public static IEnumerable<MediaEntry> WhereInPlex(this IEnumerable<MediaEntry> entries)
        {
            return entries.Where(entry => entry.ReconciledEntries.Any(entry1 => entry1 is PlexEntry));
        }

        public static IEnumerable<MediaEntry> WhereWatchedByAccounts(this IEnumerable<MediaEntry> entries, IEnumerable<int> accountIds)
        {
            if (accountIds == null)
            {
                throw new ArgumentNullException(nameof(accountIds));
            }

            var accountIdList = accountIds.ToList();

            if (!accountIdList.Any())
            {
                throw new InvalidOperationException("At least one account ID must be specified.");
            }

            return entries.Where(mediaEntry => 
                mediaEntry.ReconciledEntries.Any(entry => 
                    entry is PlexEntry plexEntry 
                    && plexEntry.WatchedByAccounts.ContainsAll(accountIdList)));
        }
    }
}
