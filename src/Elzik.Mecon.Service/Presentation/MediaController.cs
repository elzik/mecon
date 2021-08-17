using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Elzik.Mecon.Framework.Application;

namespace Elzik.Mecon.Service.Presentation
{
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IReconciledMedia _reconciledMedia;

        public MediaController(IReconciledMedia reconciledMedia)
        {
            _reconciledMedia = reconciledMedia;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string folderDefinitionName)
        {
            var largeMatroskaEntries = await _reconciledMedia.GetMediaEntries(folderDefinitionName);

            return new OkObjectResult(largeMatroskaEntries);
        }
    }
}
