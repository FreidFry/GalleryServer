using Gallery.Server.Features.Image.Dto;
using Gallery.Server.Features.Image.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto uploadDto)
        {
            var result = await _imageService.Upload(uploadDto, HttpContext);
            return result;
        }

        [HttpGet("getall/{targetUid}")]
        [Authorize]
        public async Task<IActionResult> GetAll(string targetUid)
        {
            var result = await _imageService.GetAll(targetUid, HttpContext);
            return Ok(result);
        }


    }
}
