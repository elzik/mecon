using System;
using System.Collections.Generic;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem
{
    public class FolderDefinition
    {
        public string FolderPath { get; set; }

        public string[] SupportedFileExtensions { get; set; }

        public bool Recurse { get; set; } = true;

        public IEnumerable<MediaType> MediaTypes { get; set; }
    }
}
