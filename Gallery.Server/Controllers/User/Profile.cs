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
            var userDto = await _usersDbContext.Users.Where(predicate: u => u.UserId.ToString() == UserId).
                Select(u => new UserGetDto
                {
                    Username = u.Username,
                    AvatarFilePath = u.AvatarFilePath,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .FirstOrDefaultAsync();

            if (userDto == null)
                return NotFound("User not found");

            return Ok(userDto);
        }
    }
}
