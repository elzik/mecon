using System.Collections;
using System.Reflection;
using CommandLine;

namespace Elzik.Mecon.Console.CommandLine;

public static class OptionConfigurationParserExtensions
{
    /// <summary>
    /// Converts <paramref name="options"/> to a representation for
    /// <see cref="Microsoft.Extensions.Configuration.CommandLine.CommandLineConfigurationProvider"/>
    /// </summary>
    /// <see cref="https://github.com/commandlineparser/commandline/issues/796"/>
    public static IEnumerable<string> ConvertToConfigurationArgs<T>(this Parser parser, T options)
        where T : class
    {
        var result = new List<string>();

        // To make the args uniform, strip any '=' and convert explicit
        // boolean parameters to implicit, i.e. just the switch.
        var args = parser.FormatCommandLineArgs(
            options,
            u => { u.UseEqualToken = false; u.SkipDefault = true; });

        var optionConfigurationProperties = GetOptionConfigurationProperties(typeof(T));
        var switchNameMappings = GetSwitchNameMappings(optionConfigurationProperties);

        PropertyInfo previousOption = null;
        var count = 0;
        var isValue = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (TryGetMappedOption(switchNameMappings, args[i], out var currentOption))
            {
                if (IsOnlyIEnumerableImplemented(currentOption))
                {
                    count = 0; // start counting the enumerable property's elements
                }
                else if (IsBool(currentOption))
                {
                    // because we removed default flags, a bool flag negates the default
                    result.Add(GetOptionConfigurationSwitch(currentOption));
                    result.Add($"{!GetBoolOptionDefault(currentOption)}");
                }
                else
                {
                    result.Add(GetOptionConfigurationSwitch(currentOption));
                }

                previousOption = currentOption;
                isValue = true;
            }
            else
            {
                if (previousOption != null && IsOnlyIEnumerableImplemented(previousOption))
                {
                    // each element of the enumerable needs its own indexed switch
                    var separator = GetOptionAttribute(previousOption).Separator;

                    if (char.IsWhiteSpace(separator))
                    {
                        result.Add(GetOptionConfigurationSwitch(previousOption) + ':' + count++);
                        result.Add(args[i]);
                    }
                    else
                    {
                        // handle separator which causes values to be a combined string
                        var argSplit = args[i].Split(separator);
                        foreach (var arg in argSplit)
                        {
                            result.Add(GetOptionConfigurationSwitch(previousOption) + ':' + count++);
                            result.Add(arg);
                        }
                    }
                }
                else // arg is an options parameter, or, neither an option nor a parameter
                {
                    if (isValue)
                    {
                        result.Add(args[i]);
                        isValue = false;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Get a mapping of each <see cref="PropertyInfo"/> in
    /// <paramref name="propertyInfos"/> to their associated switch names
    /// </summary>
    private static IDictionary<PropertyInfo, IEnumerable<string>> GetSwitchNameMappings(IEnumerable<PropertyInfo> propertyInfos)
    {
        var result = new Dictionary<PropertyInfo, IEnumerable<string>>();

        foreach (var propertyInfo in propertyInfos)
        {
            var switchNames = GetSwitchNames(propertyInfo);
            result.Add(propertyInfo, switchNames);
        }

        return result;
    }

    /// <summary>
    /// Gets a sequence of switch names from <paramref name="propertyInfo"/>'s
    /// attribute of <see cref="OptionAttribute"/>
    /// </summary>
    private static IEnumerable<string> GetSwitchNames(PropertyInfo propertyInfo)
    {
        var switchNames = new List<string>();

        var optionAttribute = GetOptionAttribute(propertyInfo);

        // if OptionAttribute.LongName is null, then the name is inferred from the property name
        // https://github.com/commandlineparser/commandline/blob/master/src/CommandLine/OptionAttribute.cs
        var longName = optionAttribute.LongName;
        if (string.IsNullOrEmpty(longName))
            longName = propertyInfo.Name;

        switchNames.Add($"--{longName}");

        var shortName = optionAttribute.ShortName;
        if (!string.IsNullOrEmpty(shortName))
            switchNames.Add($"-{shortName}");

        return switchNames;
    }

    /// <summary>
    /// Tests whether the property's type implements
    /// <see cref="IEnumerable"/> and that is the only
    /// interface implemented.
    /// </summary>
    private static bool IsOnlyIEnumerableImplemented(PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo.PropertyType;

        return propertyType.GetInterfaces().Length == 1
               && typeof(IEnumerable).IsAssignableFrom(propertyType);
    }

    /// <summary>
    /// Tests whether the property's type is <see cref="bool"/>
    /// </summary>
    private static bool IsBool(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType == typeof(bool);
    }

    /// <summary>
    /// Gets the properties of <paramref name="type"/> which are decorated
    /// with both <see cref="OptionAttribute"/> and
    /// <see cref="OptionConfigurationAttribute"/>.
    /// </summary>
    private static IEnumerable<PropertyInfo> GetOptionConfigurationProperties(Type type)
    {

        var properties = type.GetProperties();
        var result = properties.Where(p =>
            p.GetCustomAttribute<OptionAttribute>() != null
            && p.GetCustomAttribute<OptionConfigurationAttribute>() != null);

        return result;
    }

    /// <summary>
    /// Gets the command-line switch representation of the property's
    /// <see cref="OptionConfigurationAttribute.ConfigurationPath"/>.
    /// </summary>
    private static string GetOptionConfigurationSwitch(PropertyInfo propertyInfo)
    {
        var optionConfigAttribute = GetOptionConfigurationAttribute(propertyInfo);
        var configurationPath = string.Join(":", optionConfigAttribute.ConfigurationPath);

        return $"--{configurationPath}";
    }

    /// <summary>
    /// Tests whether <paramref name="arg"/> is a mapped value in
    /// <paramref name="switchMappings"/> and if it is, gets the
    /// <see cref="PropertyInfo"/> key.
    /// </summary>
    private static bool TryGetMappedOption(
        IDictionary<PropertyInfo, IEnumerable<string>> switchMappings,
        string arg,
        out PropertyInfo propertyInfo)
    {
        propertyInfo = null;

        foreach (var switchMapping in switchMappings)
        {
            if (switchMapping.Value.Any(s => s == arg))
            {
                propertyInfo = switchMapping.Key;
                return true;
            }
        }

        return false;
    }

    private static bool GetBoolOptionDefault(PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType != typeof(bool))
            throw new ArgumentException("Property is not of type bool", nameof(propertyInfo));

        var optionAttribute = GetOptionAttribute(propertyInfo);

        var value = optionAttribute.Default as bool?;
        return value ?? default;
    }

    private static OptionAttribute GetOptionAttribute(PropertyInfo propertyInfo)
    {
        var optionAttribute = propertyInfo.GetCustomAttribute<OptionAttribute>();
        if (optionAttribute == null)
            throw new ArgumentException(
                $"Property does not have attribute {nameof(OptionAttribute)}", nameof(propertyInfo));

        return optionAttribute;
    }

    private static OptionConfigurationAttribute GetOptionConfigurationAttribute(PropertyInfo propertyInfo)
    {
        var optionConfigAttribute = propertyInfo.GetCustomAttribute<OptionConfigurationAttribute>();
        if (optionConfigAttribute == null)
            throw new ArgumentException(
                $"Property does not have attribute {nameof(OptionConfigurationAttribute)}", nameof(propertyInfo));

        return optionConfigAttribute;
    }
}