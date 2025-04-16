using Gallery.Server.Features.Image.Dto;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Image.Services
{
    public interface IImageService
    {
        Task<IActionResult> Upload(ImageUploadDto UploadDto, HttpContext httpContext);
        Task<IEnumerable<ImageModel>> GetAll(string TargetUid, HttpContext httpContext);
    }
}
