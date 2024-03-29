﻿using Elzik.Mecon.Framework.Domain.FileSystem;
using System.Collections.Generic;

namespace Elzik.Mecon.Framework.Infrastructure.FileSystem.Options
{
    public class FileSystemOptions
    {
        public Dictionary<string, DirectoryDefinition> DirectoryDefinitions { get; set; }
    }
}
