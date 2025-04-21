using FluentValidation;
using Gallery.Server.Core.Interfaces;
using Gallery.Server.Features.Image.DTOs;
using Gallery.Server.Infrastructure.Persistence.db;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Gallery.Server.Features.Image.Services
{
    public class ImageService(AppDbContext dbContext, IValidator<ImageUploadDto> validator, IFileStorage fileStorage) : IImageService
    {
        private readonly AppDbContext _AppDbContext = dbContext;
        private readonly IValidator<ImageUploadDto> _validator = validator;
        private readonly IFileStorage _fileStorage = fileStorage;

        public async Task<IActionResult> Upload(ImageUploadDto request, HttpContext httpContext)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return new BadRequestObjectResult(validationResult.Errors);

            var userId = httpContext.User.FindFirstValue("uid");
            if (!Guid.TryParse(userId, out Guid userGuid))
                return new BadRequestObjectResult("Invalid user ID.");
            var user = await _AppDbContext.Users.FindAsync(userGuid);
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var Image = ImageModel.Create(
                request.Name ?? string.Empty,
                request.Image.FileName,
                request.Description,
                request.Publicity,
                user
            );

            var filepath = await _fileStorage.SaveFileAsync(request.Image, $"Data/UsersData/{user.UserId}/Gallery", Image.Name);
            Image.RealImagePath = filepath;

            Image.ImageUrl = _fileStorage.GetFilePath(filepath);

            await _AppDbContext.Images.AddAsync(Image);
            await _AppDbContext.SaveChangesAsync();
            return new OkResult();

        }

        public async Task<IEnumerable<ImageGetDto>> GetAll(string TargetUid, string SortBy, string OrderBy, HttpContext httpContext)
        {
            if (!Guid.TryParse(TargetUid, out Guid targetUserId))
                return [];

            var query = _AppDbContext.Images.AsQueryable();

            if (string.IsNullOrEmpty(SortBy))
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
                if (File.Exists(image.RealImagePath))
                    File.Delete(image.RealImagePath);
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
