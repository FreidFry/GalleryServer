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

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? SearchString)
        {
            if (string.IsNullOrEmpty(SearchString))
                return BadRequest("Search string cannot be null or empty.");
            var users = await _profileService.SearchAsync(SearchString);
            return Ok(users);
        }

        [HttpPut("updateavatar")]
        [Authorize]
        public async Task<IActionResult> UpdateAvatar([FromForm] UpdateProfileAvatar userUpdateDto)
        {
            if (userUpdateDto == null)
                return BadRequest("User update DTO cannot be null.");
            _profileService.UpdateProfileAvatar(userUpdateDto, HttpContext);
            return Ok();
        }
    }
}
