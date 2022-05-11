using Microsoft.Extensions.Configuration;

namespace Elzik.Mecon.Console.CommandLine.Config
{
    public static class ConfigHandler
    {
        public static void Display(ConfigurationManager config)
        {
            System.Console.WriteLine(config.ToJsonString());
        }
    }
}
