namespace Gallery.Server.Features.User.DTO
{
    public class UserGetDto
    {
        public string Username { get; set; }
        public string AvatarFilePath { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
