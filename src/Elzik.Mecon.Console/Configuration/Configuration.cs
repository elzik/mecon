using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elzik.Mecon.Console.CommandLine;
using Microsoft.Extensions.Configuration;

namespace Elzik.Mecon.Console.Configuration
{
    internal static class Configuration
    {
        public static ConfigurationManager Get()
        {
            var config = new ConfigurationManager();

            config.AddJsonFile("appsettings.json", true);
#if DEBUG
            config.AddJsonFile("appsettings.Development.json", true);
#endif
            var environmentArgs = Environment.GetCommandLineArgs();
            var args = environmentArgs.TakeLast(environmentArgs.Length - 1).ToArray();
            config.AddCommandLineParser(args);

            return config;
        }
    }
}
