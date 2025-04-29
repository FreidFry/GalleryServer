using Gallery.Server.Features.Profile.DTOs;
using Gallery.Server.Features.User.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Profile.Services
{
    public interface IProfileService
    {
        Task<UserGetDto> GetByIdAsync(string UserId, CancellationToken cancellationToken);
        Task<UserGetDto> GetCurrentAsync(HttpContext httpContext, CancellationToken cancellationToken);
        Task<IEnumerable<UserGetDto>> SearchAsync(string SearchString, CancellationToken cancellationToken);
        Task<IActionResult> UpdateProfileAvatarAsync(UpdateProfileAvatar userUpdateDto, HttpContext httpContext, CancellationToken cancellationToken);
    }
}