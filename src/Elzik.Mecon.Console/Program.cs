using CommandLine;
using Elzik.Mecon.Console;
using Elzik.Mecon.Console.CommandLine.Config;
using Elzik.Mecon.Console.CommandLine.Error;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Microsoft.Extensions.DependencyInjection;

var commandParser = new Parser(setting =>
{
    setting.CaseInsensitiveEnumValues = true;
});
var parserResult = commandParser.ParseArguments<ReconciliationOptions, ConfigOptions>(args);

parserResult
    .WithParsed<ReconciliationOptions>(options =>
    {
        var config = Configuration.Get(args);
        var services = Services.Get(config);
        var reconciliationHandler = services.GetRequiredService<IReconciliationHandler>();
        reconciliationHandler.Handle(config, options);
    })
    .WithParsed<ConfigOptions>(_ => ConfigHandler.Display(Configuration.Get(args)))
    .WithNotParsed(errors => ErrorHandler.Display(parserResult, errors));
