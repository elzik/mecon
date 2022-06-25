using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation;

public interface IOutputOperations
{
    IEnumerable<MediaEntry> PerformOutputFilters(IEnumerable<MediaEntry> entries, ReconciliationOptions options);
}