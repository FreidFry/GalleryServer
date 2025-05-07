using Gallery.Server.Features.Image.DTOs;
using Gallery.Server.Features.Image.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Gallery.Server.Controllers
{
    [EnableRateLimiting("Global")]
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
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto uploadDto, CancellationToken cancellationToken)
        {
            var result = await _imageService.UploadImageAsync(uploadDto, HttpContext, cancellationToken);
            return result;
        }

        [HttpGet("getall/{targetUid}")]
        public async Task<IActionResult> GetAll([FromRoute] string targetUid, [FromQuery] string? SortBy, [FromQuery] string? OrderBy, CancellationToken cancellationToken)
        {
            var result = await _imageService.GetImagesForUserAsync(
                targetUid,
                SortBy ?? string.Empty,
                OrderBy ?? string.Empty,
                HttpContext);
            return Ok(result);
        }

        [HttpDelete("remove")]
        [Authorize]
        public async Task<IActionResult> Remove([FromBody] List<string> imagesId, CancellationToken cancellationToken)
        {
            var result = await _imageService.RemoveImageAsync(imagesId, HttpContext, cancellationToken);
            return result;
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ImageUpdateDto updateDto, CancellationToken cancellationToken)
        {
            var result = await _imageService.UpdateImageInfoAsync(updateDto, HttpContext, cancellationToken);
            return result;
        }

        [HttpGet("random")]
        [AllowAnonymous]
        [ResponseCache(Duration = 60 * 5)]
        public async Task<ActionResult<IEnumerable<ImageGetDto>>> GetRandomImages(
            [FromQuery] int page,
            [FromQuery] int count,
            [FromQuery] string[] excludeIds,
            CancellationToken cancellationToken)
        {
            var images = await _imageService.GetRandomPublicImagesAsync(page, count, excludeIds, HttpContext, cancellationToken);
            return Ok(images);
        }
    }
}
