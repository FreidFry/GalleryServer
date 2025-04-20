using Gallery.Server.Features.Image.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Image.Services
{
    public interface IImageService
    {
        Task<IActionResult> Upload(ImageUploadDto UploadDto, HttpContext httpContext);
        Task<IEnumerable<ImageGetDto>> GetAll(string TargetUid, string SortBy, string OrderBy, HttpContext httpContext);
        Task<IActionResult> Remove(IEnumerable<string> ImageId, HttpContext httpContext);
        Task<IActionResult> Update(ImageUpdateDto UpdateDto, HttpContext httpContext);
    }
}