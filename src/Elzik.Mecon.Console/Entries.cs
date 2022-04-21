using Elzik.Mecon.Console.CommandLine;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console
{
    internal static class Entries
    {
        internal static IEnumerable<MediaEntry> PerformOutputFilters(this IEnumerable<MediaEntry> entries, ReconciliationOptions options)
        {
            if (options.MissingFromLibrary) 
                entries = entries.WhereNotInPlex();

            if (options.PresentInLibrary) 
                entries = entries.WhereInPlex();

            return entries;
        }
    }
}
