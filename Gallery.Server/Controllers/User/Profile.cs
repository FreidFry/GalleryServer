using Gallery.Server.Data.db;
using Gallery.Server.Models.User.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Server.Controllers.User
{
    [Route("[controller]")]
    [ApiController]
    public class Profile : Controller
    {
        private readonly UsersDbContext _usersDbContext;

        public Profile(UsersDbContext usersDbContext)
        {
            _usersDbContext = usersDbContext;
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> Get([FromRoute] string UserId)
        {
            var user = await _usersDbContext.Users
                .FirstOrDefaultAsync(u => u.UserId.ToString() == UserId);

            if (user == null)
                return NotFound("User not found");

            var userDto = new UserGetDto
            {
                Username = user.Username,
                AvatarFilePath = user.AvatarFilePath,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            };

            return Ok(userDto);
        }
    }
}
