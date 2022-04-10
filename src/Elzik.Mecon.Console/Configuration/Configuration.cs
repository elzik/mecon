using System.Reflection;
using Elzik.Mecon.Console.CommandLine;
using Microsoft.Extensions.Configuration;

namespace Elzik.Mecon.Console.Configuration
{
    internal static class Configuration
    {
        public static ConfigurationManager Get()
        {
            var config = new ConfigurationManager();

            config.AddJsonStream(Assembly.GetCallingAssembly()
                .GetManifestResourceStream("Elzik.Mecon.Console.appsettings.Default.json"));
            config.AddJsonFile("appsettings.json", true);
#if DEBUG
            config.AddJsonFile("appsettings.Development.json", true);
#endif
            config.AddCommandLineParser(Environment.GetCommandLineArgs());

            return config;
        }
    }
}
