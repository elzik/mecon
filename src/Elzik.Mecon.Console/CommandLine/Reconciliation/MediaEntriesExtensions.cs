using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{
    internal static class MediaEntriesExtensions
    {
        internal static IEnumerable<MediaEntry> PerformOutputFilters(this IEnumerable<MediaEntry> entries, ReconciliationOptions options)
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
