using Gallery.Server.Features.Profile.DTOs;
using Gallery.Server.Features.User.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Profile.Services
{
    public interface IProfileService
    {
        Task<UserGetDto> GetByIdAsync(string UserId);
        Task<UserGetDto> GetCurrentAsync(HttpContext httpContext);
        Task<IEnumerable<UserGetDto>> SearchAsync(string SearchString);
        Task<IActionResult> UpdateProfileAvatarAsync(UpdateProfileAvatar userUpdateDto, HttpContext httpContext);
    }
}