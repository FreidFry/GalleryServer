using Gallery.Server.Features.Image.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Image.Services
{
    public interface IImageService
    {
        Task<IActionResult> UploadImageAsync(ImageUploadDto UploadDto, HttpContext httpContext, CancellationToken cancellationToken);
        Task<IEnumerable<ImageGetDto>> GetImagesForUserAsync(string TargetUid, string SortBy, string OrderBy, HttpContext httpContext);
        Task<IActionResult> RemoveImageAsync(IEnumerable<string> ImageId, HttpContext httpContext, CancellationToken cancellationToken);
        Task<IActionResult> UpdateImageInfoAsync(ImageUpdateDto UpdateDto, HttpContext httpContext, CancellationToken cancellationToken);
        Task<IEnumerable<ImageGetDto>> GetRandomPublicImagesAsync(int page, int count, string[] excludeIds, HttpContext httpContext, CancellationToken cancellationToken);
    }
}