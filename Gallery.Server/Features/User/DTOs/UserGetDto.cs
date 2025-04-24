namespace Gallery.Server.Features.User.DTO
{
    public class UserGetDto
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string AvatarFilePath { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
