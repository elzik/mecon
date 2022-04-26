using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        public static IEnumerable<MediaEntry> WhereMatchRegex(this IEnumerable<MediaEntry> entries, string regexPattern)
        {

            var regex = new Regex(regexPattern, RegexOptions.Compiled);

            return entries.Where(entry => regex.IsMatch(entry.FilesystemEntry.FileSystemPath));
        }

        public static IEnumerable<MediaEntry> WhereNoMatchRegex(this IEnumerable<MediaEntry> entries, string regexPattern)
        {

            var regex = new Regex(regexPattern, RegexOptions.Compiled);

            return entries.Where(entry => !regex.IsMatch(entry.FilesystemEntry.FileSystemPath));
        }
    }
}
