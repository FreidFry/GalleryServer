﻿using Microsoft.AspNetCore.Mvc;
using Gallery.Server.Features.User.DTO;
using Gallery.Server.Features.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Gallery.Server.Controllers
{
    [EnableRateLimiting("Login")]
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
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(userDto, HttpContext, cancellationToken);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto, CancellationToken cancellationToken)
        {
            return await _authService.Login(userDto, HttpContext, cancellationToken);
        }

        [HttpPost("logout")]
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