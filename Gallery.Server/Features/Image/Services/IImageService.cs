using Gallery.Server.Features.Image.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Image.Services
{
    public interface IImageService
    {
        Task<IActionResult> Upload(ImageUploadDto UploadDto, HttpContext httpContext);
        Task<IEnumerable<ImageGetDto>> GetAll(string TargetUid, HttpContext httpContext);
    }
}
