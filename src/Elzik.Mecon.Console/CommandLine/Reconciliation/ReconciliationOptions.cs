using CommandLine;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Console.CommandLine.Reconciliation
{
    [Verb("reconcile", isDefault: true, 
        HelpText = "Reconcile media in a directory with media in Plex. " +
                   "This verb is only explicitly required when used in conjunction with the --help option for getting help.")]
    public class ReconciliationOptions
    {
        [OptionConfiguration( "Plex", "BaseUrl")]
        [Option('p', "plex-host", 
            HelpText = "Plex host and port.")]
        public string? PlexBaseUrl { get; set; }

        [OptionConfiguration("Plex", "AuthToken")]
        [Option('t', "plex-token",
            HelpText = "Plex token.")]
        public string? PlexToken { get; set; }


        [Option('d', "directory", Default = ".", Group = "file system source",
            HelpText = "Directory of media files to reconcile.")]
        public string? DirectoryPath { get; set; }

        [Option('e', "file-extensions", Separator = ',', Default = null,
            HelpText = "Comma-separated list of file extensions to include when searching for files. " +
                       "This option is only valid when the -d option (--directory-definition-name) is supplied. " +
                       "If this is omitted, all files will be found.")]
        public IEnumerable<string>? FileExtensions { get; set; }

        [Option('n', "directory-definition-name", Group = "file system source",
            HelpText = "Definition name of directory of media files to reconcile.")]
        public string? DirectoryKey { get; set; }

        [Option('r', "recurse", Default = true,
            HelpText = "When finding all files in directory perform a recursive search.")]
        public bool? Recurse { get; set; }


        [Option('m', "media-types", Separator = ',',
            HelpText = "Comma-separated list of media types that the specified directory contains in order to avoid " +
                       "searching through libraries that contain other media types. " +
                       "Possible options are 'Movie' or 'TvShow'. " +
                       "This option is only valid when the -d option (--directory) is supplied. " +
                       "If this is omitted, all libraries of all media types will be searched.")]
        public IEnumerable<MediaType>? MediaTypes { get; set; }

        [Option('L', "missing-from-library", Group = "required output filter",
            HelpText = "Filter output to only show files missing from media library.")]
        public bool MissingFromLibrary { get; set; }

        [Option('l', "present-in-library", Group = "required output filter",
            HelpText = "Filter output to only show files present in media library.")]
        public bool PresentInLibrary { get; set; }

        [Option('w', "watched-by", Separator = ',', Required = false ,Group = "required output filter",
            HelpText = "Filter output to only show files which have been watched by the supplied comma-separated list of user titles. If the option is supplied without a list of user titles then only files which have been watched by all users are returned.")]
        public IEnumerable<string>? WatchedByUsers { get; set; }

        [Option('f', "regex-match-filter",
            HelpText = "Filter output to only show files where the filepath matches a regular expression.")]
        public string? MatchRegex { get; set; }

        [Option('F', "regex-no-match-filter",
            HelpText = "Filter output to only show files where the filepath does not match a regular expression.")]
        public string? NoMatchRegex { get; set; }
    }
}
