using System;
using System.Collections.Generic;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Tests.Unit.Infrastructure.FileSystemTests;

namespace Elzik.Mecon.Framework.Tests.Unit.Application
{
    public static class Aligner
    {
        public static void AlignFileSystemWithPlexMediaContainer(IList<TestFileInfoImplementation> files, IList<PlexEntry> plexEntries)
        {
            if (files.Count != plexEntries.Count)
            {
                throw new InvalidOperationException(
                    "The list of files must be the same length as the list of plex entries.");
            }

            for (var i = 0; i < files.Count; i++)
            {
                plexEntries[i].Key = new EntryKey(files[i].Name, files[i].Length);
            }
        }
    }
}
