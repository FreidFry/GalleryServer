using Microsoft.AspNetCore.Mvc;

namespace Gallery.Server.Features.Profile.DTOs
{
    public class UpdateProfileAvatar
    {
        public Guid UserId { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
