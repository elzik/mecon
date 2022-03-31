using Elzik.Mecon.Console.CommandLine;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console
{
    internal static class Entries
    {
        internal static IEnumerable<MediaEntry> PerformOutputFilters(this IEnumerable<MediaEntry> entries, MeconOptions options)
        {
            if (options.MissingFromLibrary)
            {
                entries = entries.WhereNotInPlex();
            }

            if (options.PresentInLibrary)
            {
                entries = entries.WhereInPlex();
            }

            return entries;
        }
    }
}
