using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation;

public interface IOutputOperations
{
    Task<IMediaEntryCollection> PerformOutputFilters(IMediaEntryCollection entries, ReconciliationOptions options);
}