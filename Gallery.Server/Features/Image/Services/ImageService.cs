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
    public class ImageService(AppDbContext dbContext, IValidator<ImageUploadDto> validator, IFileStorage fileStorage, IHttpContextHelper httpContextHelper) : IImageService
    {
        private readonly AppDbContext _AppDbContext = dbContext;
        private readonly IValidator<ImageUploadDto> _validator = validator;
        private readonly IFileStorage _fileStorage = fileStorage;
        private readonly IHttpContextHelper _httpContextHelper = httpContextHelper;

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

            var filepath = await _fileStorage.SaveFileAsync(request.Image, _fileStorage.GetFilePath("Gallery", userGuid), Image.Name);
            Image.RealImagePath = filepath;

            Image.ImageUrl = _fileStorage.GetFileUrl("Gallery", filepath, userGuid);

            await _AppDbContext.Images.AddAsync(Image);
            await _AppDbContext.SaveChangesAsync();
            return new OkResult();
        }

        public async Task<IEnumerable<ImageGetDto>> GetAll(string TargetUid, string SortBy, string OrderBy, HttpContext httpContext)
        {
            if (!Guid.TryParse(TargetUid, out Guid targetUserId))
                return [];

            SortBy = string.IsNullOrEmpty(SortBy) ? "CreateAt" : SortBy;
            OrderBy = string.IsNullOrEmpty(OrderBy) ? "desc" : OrderBy.ToLower();

            try
            {
                var query = _AppDbContext.Images.Where(i => i.UserId == targetUserId);

                var cookieId = httpContext.User.FindFirstValue("uid");
                var isOwner = Guid.TryParse(cookieId, out Guid cookieUid) && targetUserId == cookieUid;

                if (!isOwner)
                {
                    query = query.Where(i => i.Publicity == true);
                }

                var propertyInfo = typeof(ImageModel).GetProperty(SortBy);
                if (propertyInfo == null)
                {
                    propertyInfo = typeof(ImageModel).GetProperty("CreateAt");
                }

                var parameter = Expression.Parameter(typeof(ImageModel), "i");
                var property = Expression.Property(parameter, propertyInfo);
                var lambda = Expression.Lambda(property, parameter);

                var methodName = OrderBy == "asc" ? "OrderBy" : "OrderByDescending";
                var methodCallExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    [typeof(ImageModel), propertyInfo.PropertyType],
                    query.Expression,
                    lambda
                );

                query = query.Provider.CreateQuery<ImageModel>(methodCallExpression);

                var images = await query
                    .Select(img => new ImageGetDto
                    {
                        ImageId = img.ImageId,
                        Name = img.Name ?? string.Empty,
                        Description = img.Description ?? string.Empty,
                        CreateAt = img.CreateAt,
                        ImageUrl = img.ImageUrl
                    })
                    .ToListAsync();

                return images;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll: {ex.Message}");
                return [];
            }
        }

        public async Task<IActionResult> Remove(IEnumerable<string> ImageId, HttpContext httpContext)
        {
            if (!_httpContextHelper.IsAuthenticated(httpContext))
                return new UnauthorizedResult();
                
            var currentUserId = _httpContextHelper.GetUserId(httpContext);
            if (!currentUserId.HasValue)
                return new UnauthorizedResult();
                
            var images = await _AppDbContext.Images
                .Where(i => ImageId.Contains(i.ImageId.ToString().ToLower()))
                .ToListAsync();
                
            if (images.Count == 0)
                return new NotFoundObjectResult("No images found for the provided IDs.");
                
            var unauthorizedImages = images.Where(img => !_httpContextHelper.IsOwner(httpContext, img.UserId)).ToList();
            if (unauthorizedImages.Any())
            {
                var unauthorizedIds = string.Join(", ", unauthorizedImages.Select(img => img.ImageId));
                return new ForbidResult($"You don't have permission to delete images: {unauthorizedIds}");
            }
            
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
            if (!_httpContextHelper.IsAuthenticated(httpContext))
                return new UnauthorizedResult();
                
            var currentUserId = _httpContextHelper.GetUserId(httpContext);
            if (!currentUserId.HasValue)
                return new UnauthorizedResult();
                
            var updateImage = await _AppDbContext.Images
                .Where(i => i.ImageId == image.ImageId)
                .FirstOrDefaultAsync();

            if (updateImage == null)
                return new NotFoundObjectResult("Image not found.");
                
            if (!_httpContextHelper.IsOwner(httpContext, updateImage.UserId))
                return new ForbidResult($"You don't have permission to update this image.");

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
