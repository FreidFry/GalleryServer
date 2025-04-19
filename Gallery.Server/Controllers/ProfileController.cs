using Gallery.Server.Features.Profile.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetById([FromRoute] string UserId)
        {
            var userDto = await _profileService.GetByIdAsync(UserId);
            if (userDto == null)
                return NotFound();
            return Ok(userDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCurrent()
        {
            var userDto = await _profileService.GetCurrentAsync(HttpContext);
            if (userDto == null)
                return NotFound();
            return Ok(userDto);
        }
    }
}
