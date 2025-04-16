using System.ComponentModel.DataAnnotations;

namespace Gallery.Server.Features.User.DTO
{
    public class UserLoginDto
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
