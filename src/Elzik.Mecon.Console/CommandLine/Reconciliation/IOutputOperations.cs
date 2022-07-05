using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation;

public interface IOutputOperations
{
    Task<MediaEntryCollection> PerformOutputFilters(MediaEntryCollection entries, ReconciliationOptions options);
}