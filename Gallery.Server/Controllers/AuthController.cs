﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gallery.Server.Data.db;
using Gallery.Server.Models.User;
using Gallery.Server.Models.User.DTO;
using Gallery.Server.Interfaces;
using Gallery.Server.Services;
using Microsoft.Extensions.Options;

namespace Gallery.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UsersDbContext _usersDbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly JwtOptions _envOptions;

        public AuthController(UsersDbContext users, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IOptions<JwtOptions> envOptions)
        {
            _usersDbContext = users;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _envOptions = envOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto UserDto)
        {
            if (!ModelState.IsValid || UserDto.Password != UserDto.ConfirmPassword)
                return BadRequest(ModelState);
            var existingUser = await _usersDbContext.Users
                .FirstOrDefaultAsync(u => u.Username == UserDto.Username);

            if (existingUser != null) return Conflict("User already exists");

            var newUser = UserModel.CreateUser(UserDto.Username, _passwordHasher.HashPassword(UserDto.Password));

            await _usersDbContext.Users.AddAsync(newUser);
            await _usersDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto UserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _usersDbContext.Users
                .FirstOrDefaultAsync(u => u.Username == UserDto.Username);

            if (user == null) return NotFound("User not found");
            if (!_passwordHasher.VerifyPassword(UserDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid password");
            }

            var expiration = DateTimeOffset.UtcNow.AddDays(_envOptions.ExpiresDays);

            var token = _jwtProvider.GenerateToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = expiration
            });

            user.UpdateLastLogin();

            _usersDbContext.Users.Update(user);
            await _usersDbContext.SaveChangesAsync();

            return Ok(new { Message = "Login successful" });
        }
    }
}
