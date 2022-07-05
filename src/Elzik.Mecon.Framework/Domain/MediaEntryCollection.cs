using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elzik.Mecon.Framework.Domain.Plex;

namespace Elzik.Mecon.Framework.Domain
{
    public class MediaEntryCollection : Collection<MediaEntry>
    {
        public MediaEntryCollection()
        {
        }

        public MediaEntryCollection(IEnumerable<MediaEntry> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            AddRange(entries);
        }

        public MediaEntryCollection WhereNotInPlex()
        {
            return new MediaEntryCollection(this.Where(entry => 
                !entry.ReconciledEntries.Any(entry1 => 
                    entry1 is PlexEntry)));
        }

        public MediaEntryCollection WhereInPlex()
        {
            return new MediaEntryCollection(this.Where(entry => 
                entry.ReconciledEntries.Any(entry1 => 
                    entry1 is PlexEntry)));
        }

        public MediaEntryCollection WhereWatchedByAccounts(IEnumerable<int> accountIds)
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

            return new MediaEntryCollection(this.Where(mediaEntry => 
                mediaEntry.ReconciledEntries.Any(entry => 
                    entry is PlexEntry plexEntry && plexEntry.WatchedByAccounts.ContainsAll(accountIdList))));
        }

        public void AddRange(IEnumerable<MediaEntry> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            foreach (var mediaEntry in values)
            {
                Add(mediaEntry);
            }
        }
    }
}
