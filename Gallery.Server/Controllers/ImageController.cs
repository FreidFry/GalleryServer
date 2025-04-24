using Gallery.Server.Features.Image.DTOs;
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
        public async Task<IActionResult> GetAll([FromRoute] string targetUid, [FromQuery] string? SortBy, [FromQuery] string? OrderBy)
        {
            var result = await _imageService.GetAll(
                targetUid,
                SortBy ?? string.Empty,
                OrderBy ?? string.Empty,
                HttpContext);
            return Ok(result);
        }

        [HttpDelete("remove")]
        [Authorize]
        public async Task<IActionResult> Remove([FromBody] List<string> imagesId)
        {
            var result = await _imageService.Remove(imagesId, HttpContext);
            return result;
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ImageUpdateDto updateDto)
        {
            var result = await _imageService.Update(updateDto, HttpContext);
            return result;
        }

    }
}
