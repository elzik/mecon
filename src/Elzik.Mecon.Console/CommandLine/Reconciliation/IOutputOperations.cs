using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation;

public interface IOutputOperations
{
    Task<IEnumerable<MediaEntry>> PerformOutputFilters(IEnumerable<MediaEntry> entries, ReconciliationOptions options);
}