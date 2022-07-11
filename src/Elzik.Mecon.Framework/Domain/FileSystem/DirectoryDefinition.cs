using System.Collections.Generic;

namespace Elzik.Mecon.Framework.Domain.FileSystem
{
    public class DirectoryDefinition
    {
        public string DirectoryPath { get; set; }

        public string DirectoryFilterRegexPattern { get; set; }

        public string[] SupportedFileExtensions { get; set; }

        public bool Recurse { get; set; } = true;

        public IEnumerable<MediaType> MediaTypes { get; set; }
    }
}
