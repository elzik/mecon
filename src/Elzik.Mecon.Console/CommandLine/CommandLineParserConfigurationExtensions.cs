﻿using CommandLine;
using Microsoft.Extensions.Configuration;

namespace Elzik.Mecon.Console.CommandLine
{
    public static class CommandLineParserConfigurationExtensions
    {
        public static IConfigurationBuilder AddCommandLineParser(
            this IConfigurationBuilder configurationBuilder,
            string[] args)
        {
            if (!args.Any()) return configurationBuilder;

            var commandParser = new Parser(setting =>
            {
                setting.CaseSensitive = false;
                setting.CaseInsensitiveEnumValues = true;
            });

            var meconOptions = commandParser.ParseArguments<MeconOptions>(args);
            var meconArgs = commandParser.ConvertToConfigurationArgs(meconOptions.Value).ToArray();

            configurationBuilder.AddCommandLine(meconArgs);

            return configurationBuilder;
        }
    }
}