using System.Collections.Generic;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Domain;
using Elzik.Mecon.Framework.Domain.FileSystem;

namespace Elzik.Mecon.Framework.Application
{
    public interface IReconciledMedia
    {
        Task<IEnumerable<MediaEntry>> GetMediaEntries(string directoryDefinitionName);

        Task<IEnumerable<MediaEntry>> GetMediaEntries(DirectoryDefinition directoryDefinition);
    }
}