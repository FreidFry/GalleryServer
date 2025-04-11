using System.ComponentModel.DataAnnotations;

namespace Gallery.Server.Models.User.DTO
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        [MinLength(6)]
        public required string ConfirmPassword { get; set; }

    }
}
