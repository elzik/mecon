﻿using System.Collections.Generic;
using System.IO.Abstractions;

namespace Elzik.Mecon.Framework.Domain.FileSystem
{
    public interface IFileSystem
    {
        DirectoryDefinition GetDirectoryDefinition(string directoryDefinitionName);

        IEnumerable<IFileInfo> GetMediaFileInfos(DirectoryDefinition directoryDefinition);
    }
}
