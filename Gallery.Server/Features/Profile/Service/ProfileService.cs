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
        private readonly AppDbContext _usersDbContext;
        private readonly IFileStorage _fileStorage;

        public ProfileService(AppDbContext appDbContext, IFileStorage fileStorage)
        {
            _usersDbContext = appDbContext;
            _fileStorage = fileStorage;
        }

        public async Task<UserGetDto> GetByIdAsync(string UserId)
        {
            UserGetDto userDto = await _usersDbContext.Users
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

            var userDto = await _usersDbContext.Users.Where(u => u.UserId.ToString().ToUpper() == userId.ToUpper()).
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
            var users = await _usersDbContext.Users
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

        public async Task<IActionResult> UpdateProfileAvatar(UpdateProfileAvatar userUpdateDto, HttpContext httpContext)
        {
            Guid userId = Guid.Parse(httpContext.User.FindFirstValue("uid"));
            if (userUpdateDto.UserId != userId) 
                return new BadRequestObjectResult("User ID mismatch.");

            var user = await _usersDbContext.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            if (userUpdateDto.Avatar.Length > 5 * 1024 * 1024) // 5MB limit
                return new BadRequestObjectResult("File is too large.");

            Console.WriteLine($"Received file: {userUpdateDto.Avatar.FileName}, Size: {userUpdateDto.Avatar.Length} bytes");

            if (!string.IsNullOrEmpty(user.AvatarFilePath) &&
                !user.AvatarFilePath.Contains("default/img/defaultUserAvatar.png"))
            {
                await _fileStorage.DeleteFileAsync(user.AvatarFilePath);
            }

            var filePath = await _fileStorage.SaveFileAsync(
                userUpdateDto.Avatar,
                _fileStorage.GetFilePath("Profile", userId),
                userUpdateDto.Avatar.FileName
            );

            user.AvatarFilePath = filePath;
            user.AvatarUrl = _fileStorage.GetFileUrl("Profile", filePath, userId);

            _usersDbContext.Users.Update(user);
            await _usersDbContext.SaveChangesAsync();
            return new OkObjectResult(user);
        }
    }
}
