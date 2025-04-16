using Microsoft.AspNetCore.Mvc;
using Gallery.Server.Features.User.DTO;
using Gallery.Server.Features.User.Services;
using Microsoft.AspNetCore.Authorization;

namespace Gallery.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            return await _authService.RegisterAsync(userDto, HttpContext);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            return await _authService.Login(userDto, HttpContext);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            _authService.Logout(HttpContext);
            return Ok(new { Message = "Logout successful" });
        }

        [HttpGet("init")]
        [Authorize]
        public IActionResult Init()
        {
            var uid = _authService.Init(User);
            return Ok(new { userId = uid });
        }
    }
}