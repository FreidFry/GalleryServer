using System.ComponentModel.DataAnnotations;

namespace Gallery.Server.Models.User.DTO
{
    public class UserLoginDto
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
