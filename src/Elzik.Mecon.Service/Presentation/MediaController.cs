using Elzik.Mecon.Service.Application;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elzik.Mecon.Service.Presentation
{
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMedia _media;

        public MediaController(IMedia media)
        {
            _media = media;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string mediaPath)
        {
            var largeMatroskaEntries = await _media.GetMediaEntries(mediaPath);

            return new OkObjectResult(largeMatroskaEntries);
        }
    }
}
