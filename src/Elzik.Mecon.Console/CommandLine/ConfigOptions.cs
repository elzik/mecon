using CommandLine;

namespace Elzik.Mecon.Console.CommandLine
{
    [Verb("config", HelpText = "Display all configuration options currently in effect.")]
    public class ConfigOptions
    {
        [Option('l', "list", Required = true,
            HelpText = "List all configuration settings set for the application from default settings, " +
                       "appSettings.json file in the executable directory and environment variables in that order of presidence. " +
                       "That is, any settings set in appSettings.json will overwrite identically named default settings and any " +
                       "identically named settings set in environment settings will overwrite those.")]
        public bool ListConfig { get; set; }
    }
}
