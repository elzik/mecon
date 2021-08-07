using System.Collections.Generic;

namespace Elzik.Mecon.Service.Domain
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

        public MediaEntry()
        {
            FilesystemEntry = new FilesystemEntry();
        }

        public FilesystemEntry FilesystemEntry { get; set; }

        public IList<IEntry> ReconciledEntries { get; set; }
    }
}