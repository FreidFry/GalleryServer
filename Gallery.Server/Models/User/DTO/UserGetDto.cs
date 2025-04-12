namespace Gallery.Server.Models.User.DTO
{
    public class UserGetDto
    {
        public string Username { get; set; }
        public string AvatarFilePath { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
