using CommandLine;

namespace Elzik.Mecon.Console.CommandLine
{
    [Verb("reconcile", isDefault: true, 
        HelpText = "Reconcile media in a directory with media in Plex.")]
    public class MeconOptions
    {
        [OptionConfiguration("Plex", "BaseUrl")]
        [Option('p', "plex-host", 
            HelpText = "Plex host and port.")]
        public string? PlexBaseUrl { get; set; }
        

        [Option('d', "directory", Group = "file system source",
            HelpText = "Directory of media files to reconcile.")]
        public string? DirectoryPath { get; set; }
         
        [Option('n', "directory-definition-name", Group = "file system source",
            HelpText = "Definition name of directory of media files to reconcile.")]
        public string? DirectoryName { get; set; }

        [Option('e', "file-extensions", Separator = ',', Default = null,
            HelpText = "Comma-separated list of file extensions to include when searching for files. This option is only valid when the -d option (--directory-definition-name) is supplied. If this is omitted, all files will be found.")]
        public IEnumerable<string>? FileExtensions { get; set; }

        [Option('r', "recurse", Default = true,
            HelpText = "When finding all files in directory perform a recursive search.")]
        public bool? Recurse { get; set; }
        

        [Option('L', "missing-from-library", Group = "output filter",
            HelpText = "Filter output to only show files missing from media library.")]
        public bool MissingFromLibrary { get; set; }

        [Option('l', "present-in-library", Group = "output filter",
            HelpText = "Filter output to only show files present in media library.")]
        public bool PresentInLibrary { get; set; }
    }
}
