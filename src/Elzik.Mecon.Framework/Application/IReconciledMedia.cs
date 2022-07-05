using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.FileSystem;

namespace Elzik.Mecon.Framework.Application
{
    public interface IReconciledMedia
    {
        Task<MediaEntryCollection> GetMediaEntries(string directoryDefinitionName);

        Task<MediaEntryCollection> GetMediaEntries(DirectoryDefinition directoryDefinition);
    }
}