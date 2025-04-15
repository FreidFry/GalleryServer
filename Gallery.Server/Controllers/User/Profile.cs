using Gallery.Server.Data.db;
using Gallery.Server.Models.User.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Server.Controllers.User
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class Profile : Controller
    {
        private readonly AppDbContext _usersDbContext;

        public Profile(AppDbContext usersDbContext)
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
