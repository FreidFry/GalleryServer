using Gallery.Server.Features.User.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gallery.Server.Features.User.Services
{
    public interface IAuthService
    {
        public Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto UserDto, HttpContext httpContext, CancellationToken cancellationToken);
        public Task<IActionResult> Login([FromBody] UserLoginDto UserDto, HttpContext httpContext, CancellationToken cancellationToken);
        public void Logout(HttpContext httpContext);
        public string Init(ClaimsPrincipal user);
    }
}
