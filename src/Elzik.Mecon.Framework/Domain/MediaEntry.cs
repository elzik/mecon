using System.Collections.Generic;

namespace Elzik.Mecon.Framework.Domain
{
    public class MediaEntry
    {
        public MediaEntry(string filePath)
        {
            FilesystemEntry = new FilesystemEntry()
            {
                FileSystemPath = filePath
            };

            ReconciledEntries = new List<IEntry>();
        }

        public FilesystemEntry FilesystemEntry { get; set; }

        public IList<IEntry> ReconciledEntries { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}