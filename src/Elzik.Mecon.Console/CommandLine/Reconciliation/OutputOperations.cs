using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{
    internal class OutputOperations : IOutputOperations
    {
        public IEnumerable<MediaEntry> PerformOutputFilters(IEnumerable<MediaEntry> entries, ReconciliationOptions options)
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
