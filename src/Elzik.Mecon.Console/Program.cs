using CommandLine;
using Elzik.Mecon.Console;
using Elzik.Mecon.Console.CommandLine.Config;
using Elzik.Mecon.Console.CommandLine.Error;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Microsoft.Extensions.DependencyInjection;

var config = Configuration.Get(args);
var services = Services.Get(config);

var commandParser = new Parser(setting =>
{
    setting.CaseInsensitiveEnumValues = true;
});

var parserResult = commandParser.ParseArguments<ReconciliationOptions, ConfigOptions>(args);

parserResult
    .WithParsed<ReconciliationOptions>(options =>
    {
        var reconciliationHandler = services.GetRequiredService<IReconciliationHandler>();
        reconciliationHandler.Handle(config, options, args);
    })
    .WithParsed<ConfigOptions>(_ => ConfigHandler.Display(config))
    .WithNotParsed(errors => ErrorHandler.Display(parserResult, errors));
