using Gallery.Server.Features.Image.DTOs;
using Gallery.Server.Infrastructure.Persistence.db;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Gallery.Server.Features.Image.Services
{
    public class ImageService(AppDbContext dbContext) : IImageService
    {
        private readonly AppDbContext _AppDbContext = dbContext;

        public async Task<IActionResult> Upload(ImageUploadDto request, HttpContext httpContext)
        {
            string userId = httpContext.User.FindFirstValue("uid")?? string.Empty;

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

        public async Task<IEnumerable<ImageGetDto>> GetAll(string TargetUid, string SortBy, string OrderBy, HttpContext httpContext)
        {
            if (!Guid.TryParse(TargetUid, out Guid targetUserId))
                return [];

            var query = _AppDbContext.Images.AsQueryable();

            if(string.IsNullOrEmpty(SortBy))
                SortBy = "CreateAt";
            if (string.IsNullOrEmpty(OrderBy))
                OrderBy = "desc";

            var CookieId = httpContext.User.FindFirstValue("uid");
            if (!Guid.TryParse(CookieId, out Guid CookieUid) || targetUserId != CookieUid)
            {
                query = query.Where(i => i.Publicity == true);
            }

            var propertyInfo = typeof(ImageModel).GetProperty(SortBy);
            if (propertyInfo == null)
                return [];

            var parameter = Expression.Parameter(typeof(ImageModel), "i");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = OrderBy.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";

            var methodCallExpressin = Expression.Call(
                typeof(Queryable),
                methodName,
                [typeof(ImageModel), propertyInfo.PropertyType],
                query.Expression,
                lambda
            );

            query = query.Provider.CreateQuery<ImageModel>(methodCallExpressin);

            var images = await query
                .Select(img => new ImageGetDto
                {
                    ImageId = img.ImageId,
                    Name = img.Name ?? string.Empty,
                    Description = img.Description ?? string.Empty,
                    CreateAt = img.CreateAt,
                    ImageUrl = img.ImageUrl
                }).ToListAsync();

            return images;
        }


        public async Task<IActionResult> Remove(IEnumerable<string> ImageId, HttpContext httpContext)
        {
            Guid userId = Guid.Parse(httpContext.User.FindFirstValue("uid") ?? string.Empty);
            var user = await _AppDbContext.Users.FindAsync(userId);
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
            Guid userId = Guid.Parse(httpContext.User.FindFirstValue("uid") ?? string.Empty);
            var updateImage = await _AppDbContext.Images
                .Where(i => i.ImageId == image.ImageId && i.UserId == userId)
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
