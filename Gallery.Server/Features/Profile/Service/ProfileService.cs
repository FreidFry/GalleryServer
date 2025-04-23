using FluentValidation;
using Gallery.Server.Core.Interfaces;
using Gallery.Server.Features.Profile.DTOs;
using Gallery.Server.Features.User.DTO;
using Gallery.Server.Infrastructure.Persistence.db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gallery.Server.Features.Profile.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _AppDbContext;
        private readonly IFileStorage _fileStorage;
        private readonly IValidator<UpdateProfileAvatar> _validator;

        public ProfileService(AppDbContext appDbContext, IFileStorage fileStorage, IValidator<UpdateProfileAvatar> validator)
        {
            _AppDbContext = appDbContext;
            _fileStorage = fileStorage;
            _validator = validator;
        }

        public async Task<UserGetDto> GetByIdAsync(string UserId)
        {
            UserGetDto userDto = await _AppDbContext.Users
                .Where(u => u.UserId.ToString().ToUpper() == UserId.ToUpper())
                .Select(u => new UserGetDto
                {
                    Username = u.Username,
                    AvatarFilePath = u.AvatarUrl,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .FirstOrDefaultAsync();

            return userDto;
        }

        public async Task<UserGetDto> GetCurrentAsync(HttpContext httpContext)
        {
            string userId = httpContext.User.FindFirstValue("uid");

            var userDto = await _AppDbContext.Users.Where(u => u.UserId.ToString().ToUpper() == userId.ToUpper()).
                Select(u => new UserGetDto
                {
                    Username = u.Username,
                    AvatarFilePath = u.AvatarUrl,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .FirstOrDefaultAsync();

            return userDto;
        }

        public async Task<IEnumerable<UserGetDto>> SearchAsync(string SearchString)
        {
            var users = await _AppDbContext.Users
                .Where(u => u.Username.ToUpper().Contains(SearchString.ToUpper()))
                .Select(u => new UserGetDto
                {
                    Id = u.UserId,
                    Username = u.Username,
                    AvatarFilePath = u.AvatarUrl,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .ToListAsync();
            return users;
        }

        public async Task<IActionResult> UpdateProfileAvatarAsync(UpdateProfileAvatar userUpdateDto, HttpContext httpContext)
        {
            var validationResult = await _validator.ValidateAsync(userUpdateDto);
            if (!validationResult.IsValid)
                return new BadRequestObjectResult(validationResult.Errors);

            var userId = httpContext.User.FindFirstValue("uid");
            if (!Guid.TryParse(userId, out Guid userGuid))
                return new BadRequestObjectResult("Invalid user ID.");

            var user = await _AppDbContext.Users.FindAsync(userGuid);
            if (user == null)
                return new NotFoundObjectResult("User not found.");
            if (user.UserId != userGuid)
                return new BadRequestObjectResult("Invalid user ID.");

            if (!string.IsNullOrEmpty(user.AvatarFilePath) &&
                !user.AvatarFilePath.Contains("default/img/defaultUserAvatar.png"))
                await _fileStorage.DeleteFileAsync(user.AvatarFilePath);

            var filePath = await _fileStorage.SaveFileAsync(
                userUpdateDto.Avatar,
                _fileStorage.GetFilePath("Profile", userGuid),
                "Avatar"
            );

            user.AvatarFilePath = filePath;
            user.AvatarUrl = _fileStorage.GetFileUrl("Profile", filePath, userGuid);

            _AppDbContext.Users.Update(user);
            await _AppDbContext.SaveChangesAsync();
            return new OkObjectResult(user);
        }
    }
}
