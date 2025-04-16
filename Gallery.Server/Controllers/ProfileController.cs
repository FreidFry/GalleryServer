using Gallery.Server.Features.User.DTO;
using Gallery.Server.Infrastructure.Persistence.db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _usersDbContext;

        public ProfileController(AppDbContext usersDbContext)
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
