using Elzik.Mecon.Service.Application;
using Microsoft.AspNetCore.Mvc;

namespace Elzik.Mecon.Service.Presentation
{
    [ApiController]
    [Route("[controller]")]
    public class MediaEntriesController : ControllerBase
    {
        private readonly IFileSystemMedia _fileSystemMedia;

        public MediaEntriesController(IFileSystemMedia fileSystemMedia)
        {
            _fileSystemMedia = fileSystemMedia;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var largeMatroskaEntries = _fileSystemMedia.GetMediaEntries();

            return new OkObjectResult(largeMatroskaEntries);
        }
    }
}
