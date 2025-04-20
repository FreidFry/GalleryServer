using Gallery.Server.Features.Profile.DTOs;
using Gallery.Server.Features.User.DTO;
using Gallery.Server.Infrastructure.Persistence.db;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gallery.Server.Features.Profile.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _usersDbContext;

        public ProfileService(AppDbContext appDbContext)
        {
            _usersDbContext = appDbContext;
        }

        public async Task<UserGetDto> GetByIdAsync(string UserId)
        {
            UserGetDto userDto = await _usersDbContext.Users
                .Where(u => u.UserId.ToString().ToUpper() == UserId.ToUpper())
                .Select(u => new UserGetDto
                {
                    Username = u.Username,
                    AvatarFilePath = u.AvatarFilePath,
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
                    AvatarFilePath = u.AvatarFilePath,
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
                    Username = u.Username,
                    AvatarFilePath = u.AvatarFilePath,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .ToListAsync();
            return users;
        }

        public async Task<IActionResult> UpdateProfileAvatar(UpdateProfileAvatar userUpdateDto, HttpContext httpContext)
        {
            Guid userId = Guid.Parse(httpContext.User.FindFirstValue("uid"));
            var user = await _usersDbContext.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var avatarUrl = user.UpdateAvatar(userUpdateDto);

            user.AvatarFilePath = avatarUrl;
            _usersDbContext.Users.Update(user);
            await _usersDbContext.SaveChangesAsync();
            return new OkObjectResult(user);
        }
    }
}
