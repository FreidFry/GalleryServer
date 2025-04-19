using Gallery.Server.Features.Image.DTOs;
using Gallery.Server.Infrastructure.Persistence.db;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gallery.Server.Features.Image.Services
{
    public class ImageService : IImageService
    {
        private readonly AppDbContext _AppDbContext;

        public ImageService(AppDbContext dbContext)
        {
            _AppDbContext = dbContext;
        }

        public async Task<IActionResult> Upload(ImageUploadDto request, HttpContext httpContext)
        {
            string userId = httpContext.User.FindFirstValue("uid");

            if (request.Image == null || request.Image.Length == 0)
                return new BadRequestObjectResult("No image file provided.");

            if (string.IsNullOrEmpty(request.Name))
                return new BadRequestObjectResult("Image name is required.");

            var user = await _AppDbContext.Users.FindAsync(Guid.Parse(userId));

            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var imageModel = ImageModel.Create(request.Name, request.Image.FileName, request.Description, request.Publicity, user);

            var dir = Path.GetDirectoryName(imageModel.ImageFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            using (var stream = new FileStream(imageModel.ImageFilePath, FileMode.Create))
                await request.Image.CopyToAsync(stream);

            _AppDbContext.Images.Add(imageModel);
            await _AppDbContext.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<IEnumerable<ImageGetDto>> GetAll(string TargetUid, HttpContext httpContext)
        {
            string cookieUid = httpContext.User.FindFirstValue("uid");
            bool isOwner = false;
            var user = await _AppDbContext.Users.FindAsync(Guid.Parse(TargetUid));

            if (cookieUid.ToUpper() == TargetUid.ToUpper())
                isOwner = true;

            if (isOwner)
            {
                var imgs = await _AppDbContext.Images.Where(u => u.UserId == Guid.Parse(TargetUid))
                    .Select(i => ImageGetDto.FromModel(i))
                    .ToListAsync();
                return imgs;
            }
            var imgsNoPublish = await _AppDbContext.Images
                .Where(x => x.UserId == Guid.Parse(TargetUid) && x.Publicity == false)
                .Select(i => ImageGetDto.FromModel(i))
                .ToListAsync();

            return imgsNoPublish;
        }

        public async Task<IActionResult> Remove(IEnumerable<string> ImageId, HttpContext httpContext)
        {
            string userId = httpContext.User.FindFirstValue("uid");
            var user = await _AppDbContext.Users.FindAsync(Guid.Parse(userId));
            if (user == null) 
                return new NotFoundObjectResult("User not found.");
            var images = await _AppDbContext.Images
                .Where(i => ImageId.Contains(i.ImageId.ToString().ToLower()) && i.UserId == user.UserId)
                .ToListAsync();
            if (images.Count == 0)
                return new NotFoundObjectResult("No images found for the provided IDs.");
            foreach (var image in images)
            {
                if (File.Exists(image.ImageFilePath))
                    File.Delete(image.ImageFilePath);
            }
            _AppDbContext.Images.RemoveRange(images);
            await _AppDbContext.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IActionResult> Update(ImageUpdateDto image, HttpContext httpContext)
        {
            string userId = httpContext.User.FindFirstValue("uid");
            var updateImage = await _AppDbContext.Images
                .Where(i => i.ImageId.ToString().ToLower() == image.ImageId.ToString().ToLower() && i.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();
            if (updateImage == null)
                return new NotFoundObjectResult("Image not found.");

            updateImage.Name = image.Name;
            updateImage.Description = image.Description;
            updateImage.Publicity = image.Publicity;
            updateImage.LastUpdate = DateTime.UtcNow;

            _AppDbContext.Images.Update(updateImage);
            await _AppDbContext.SaveChangesAsync();
            return new OkResult();
        }
    }
}
