namespace Elzik.Mecon.Console.CommandLine
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class OptionConfigurationAttribute : Attribute
    {
        /// <summary>
        /// A sequence of configuration section names ending with the leaf
        /// configuration value name.
        /// </summary>
        /// <see cref="https://github.com/commandlineparser/commandline/issues/796"/>
        public string[] ConfigurationPath { get; }

        public OptionConfigurationAttribute(params string[] configurationPath)
        {
            ConfigurationPath = configurationPath;
        }
    }
}
