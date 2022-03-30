﻿using CommandLine;

namespace Elzik.Mecon.Console.CommandLine
{
    [Verb("ReconcilePlex", isDefault: true, 
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
    }
}
