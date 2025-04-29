using Gallery.Server.Features.Profile.DTOs;
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
        public async Task<IActionResult> GetById([FromRoute] string UserId, CancellationToken cancellationToken)
        {
            var userDto = await _profileService.GetByIdAsync(UserId, cancellationToken);
            if (userDto == null)
                return NotFound();
            return Ok(userDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            var userDto = await _profileService.GetCurrentAsync(HttpContext, cancellationToken);
            if (userDto == null)
                return NotFound();
            return Ok(userDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? SearchString, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(SearchString))
                return BadRequest("Search string cannot be null or empty.");
            var users = await _profileService.SearchAsync(SearchString, cancellationToken);
            return Ok(users);
        }

        [HttpPut("updateavatar")]
        [Authorize]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateProfileAvatar userUpdateDto, CancellationToken cancellationToken)
        {
            if (userUpdateDto == null)
                return BadRequest("User update DTO cannot be null.");
            await _profileService.UpdateProfileAvatarAsync(userUpdateDto, HttpContext, cancellationToken);
            return Ok();
        }
    }
}
