using Microsoft.Extensions.Configuration;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation;

public interface IReconciliationHandler
{
    void Handle(IConfigurationBuilder configurationBuilder, ReconciliationOptions reconciliationOptions);
}