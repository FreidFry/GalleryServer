namespace Gallery.Server.Features.Profile.DTOs
{
    public class UpdateProfileAvatar
    {
        public Guid UserId { get; set; }
        public required IFormFile Avatar { get; set; }
    }
}
