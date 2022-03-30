using CommandLine;

namespace Elzik.Mecon.Console.CommandLine
{
    [Verb("ReconcilePlex", isDefault: true, HelpText = "Reconcile media in a directory with media in Plex.")]
    public class MeconOptions
    {
        [OptionConfiguration("Plex", "BaseUrl")]
        [Option('p', "plex-host", Required = true, HelpText = "Plex host and port.")]
        public string? PlexBaseUrl { get; set; }
    }
}
