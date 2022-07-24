using CommandLine;
using Elzik.Mecon.Console;
using Elzik.Mecon.Console.CommandLine.Config;
using Elzik.Mecon.Console.CommandLine.Error;
using Elzik.Mecon.Console.CommandLine.Reconciliation;
using Elzik.Mecon.Console.CommandLine.Users;
using Microsoft.Extensions.DependencyInjection;

AppDomain currentDomain = AppDomain.CurrentDomain;
currentDomain.UnhandledException += HandleException;

var commandParser = new Parser(setting =>
{
    setting.CaseInsensitiveEnumValues = true;
});
var parserResult = commandParser.ParseArguments<ReconciliationOptions, ConfigOptions, UsersOptions>(args);

parserResult
    .WithParsed<ReconciliationOptions>(options =>
    {
        var config = Configuration.Get(args);
        var services = Services.Get(config);
        var reconciliationHandler = services.GetRequiredService<IReconciliationHandler>();
        reconciliationHandler.Handle(config, options);
    })
    .WithParsed<ConfigOptions>(_ => ConfigHandler.Display(Configuration.Get(args)))
    .WithParsed<UsersOptions>(options =>
    {
        var config = Configuration.Get(args);
        var services = Services.Get(config);
        var usersHandler = services.GetRequiredService<IUsersHandler>();
        usersHandler.DisplayUsers();
    })
    .WithNotParsed(errors => ErrorHandler.Display(parserResult, errors));

static void HandleException(object sender, UnhandledExceptionEventArgs args)
{
    Exception e = (Exception)args.ExceptionObject;
    Console.WriteLine($"Error: {e.Message}");
    Environment.Exit(1);
}