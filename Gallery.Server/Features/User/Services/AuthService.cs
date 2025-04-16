using Gallery.Server.Core.Interfaces;
using Gallery.Server.Core.Services;
using Gallery.Server.Features.User.DTO;
using Gallery.Server.Infrastructure.Persistence.db;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace Gallery.Server.Features.User.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly JwtOptions _envOptions;

        public AuthService(AppDbContext appDbContext, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IOptions<JwtOptions> envOptions)
        {
            _appDbContext = appDbContext;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _envOptions = envOptions.Value;
        }

        public async Task<IActionResult> RegisterAsync(UserRegisterDto UserDto, HttpContext httpContext)
        {
            if (UserDto.Password != UserDto.ConfirmPassword)
                return new BadRequestObjectResult("Passwords do not match");
            var existingUser = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Username == UserDto.Username);

            if (existingUser != null) return new ConflictObjectResult("User already exists");

            var newUser = UserModel.CreateUser(UserDto.Username, _passwordHasher.HashPassword(UserDto.Password));

            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();

            SetJwtCookie(httpContext, newUser);
            newUser.UpdateLastLogin();

            return new OkResult();
        }

        public async Task<IActionResult> Login(UserLoginDto UserDto, HttpContext httpContext)
        {
            var user = await _appDbContext.Users
                .FirstOrDefaultAsync(u => u.Username == UserDto.Username);

            if (user == null) return new NotFoundObjectResult("User not found");
            if (!_passwordHasher.VerifyPassword(UserDto.Password, user.PasswordHash))
            {
                return new UnauthorizedObjectResult("Invalid password");
            }

            SetJwtCookie(httpContext, user);
            user.UpdateLastLogin();

            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync();

            return new OkObjectResult(new { Message = "Login successful" });
        }

        public void Logout(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("jwt");
        }

        public string Init(ClaimsPrincipal user)
        {
            return user.FindFirstValue("uid");
        }

        void SetJwtCookie(HttpContext http, UserModel user)
        {
            var token = _jwtProvider.GenerateToken(user);
            var expiration = DateTimeOffset.UtcNow.AddDays(_envOptions.ExpiresDays);

            http.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = expiration
            });
        }
    }
}
