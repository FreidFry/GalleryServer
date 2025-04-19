using Gallery.Server.Features.User.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Profile.Services
{
    public interface IProfileService
    {
        Task<UserGetDto> GetByIdAsync(string UserId);
        Task<UserGetDto> GetCurrentAsync(HttpContext httpContext);
    }
}