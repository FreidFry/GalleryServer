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
    }
}
