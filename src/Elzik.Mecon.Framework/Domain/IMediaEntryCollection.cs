using System.Collections.Generic;

namespace Elzik.Mecon.Framework.Domain;

public interface IMediaEntryCollection : ICollection<MediaEntry>
{
    MediaEntryCollection WhereNotInPlex();
    MediaEntryCollection WhereInPlex();
    MediaEntryCollection WhereWatchedByAccounts(IEnumerable<int> accountIds);
}