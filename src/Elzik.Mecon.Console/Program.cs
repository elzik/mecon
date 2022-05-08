using CommandLine;
using Elzik.Mecon.Console.CommandLine.Config;
using Elzik.Mecon.Console.CommandLine.Error;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Console.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = Configuration.Get();
var services = Services.Get(config);

var reconciliationHandler = services.GetRequiredService<IReconciliationHandler>();

var commandParser = new Parser(setting =>
{
    setting.CaseInsensitiveEnumValues = true;
});

var parserResult = commandParser.ParseArguments<ReconciliationOptions, ConfigOptions>(args);

parserResult
    .WithParsed<ReconciliationOptions>(options => reconciliationHandler.Handle(config, options))
    .WithParsed<ConfigOptions>(_ => ConfigHandler.Display(config))
    .WithNotParsed(errors => ErrorHandler.Display(parserResult, errors));
