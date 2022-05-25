using System.Reflection;
using Elzik.Mecon.Console.CommandLine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Elzik.Mecon.Console
{
    public static class Configuration
    {
        public static ConfigurationManager Get(string[] args)
        {
            var config = new ConfigurationManager();

            config.AddJsonStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Elzik.Mecon.Console.appsettings.Default.json"));
            config.AddJsonFile("appsettings.json", true);
#if DEBUG
            config.AddJsonFile("appsettings.Development.json", true);
#endif
            config.AddEnvironmentVariables("Mecon:");
            config.AddCommandLineParser(args);

            return config;
        }

        public static string ToJsonString(this IConfiguration config)
        {
            return Serialise(config).ToString();
        }

        private static JToken Serialise(IConfiguration config)
        {
            var jObject = new JObject();

            foreach (var child in config.GetChildren())
            {
                if (child.Path.StartsWith("Logging") || child.Path.StartsWith("FileSystem") || child.Path.StartsWith("Plex"))
                {
                    if (child.Path.EndsWith(":0"))
                    {
                        var arr = new JArray();

                        foreach (var arrayChild in config.GetChildren())
                        {
                            arr.Add(Serialise(arrayChild));
                        }

                        return arr;
                    }

                    if (child.Key.Equals("AuthToken", StringComparison.InvariantCultureIgnoreCase))
                    {
                        child.Value = "<set-but-redacted>";
                    }

                    jObject.Add(child.Key, Serialise(child));
                }
            }

            if (!jObject.HasValues && config is IConfigurationSection section)
            {
                if (bool.TryParse(section.Value, out bool booleanValue))
                {
                    return new JValue(booleanValue);
                }

                if (decimal.TryParse(section.Value, out decimal decimalValue))
                {
                    return new JValue(decimalValue);
                }

                if (long.TryParse(section.Value, out long longValue))
                {
                    return new JValue(longValue);
                }

                return new JValue(section.Value);
            }

            return jObject;
        }
    }
}
