using Elzik.Mecon.Service.Application;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Get([FromQuery]string mediaPath)
        {
            var largeMatroskaEntries = await _reconciledMedia.GetMediaEntries(mediaPath);

            return new OkObjectResult(largeMatroskaEntries);
        }
    }
}
