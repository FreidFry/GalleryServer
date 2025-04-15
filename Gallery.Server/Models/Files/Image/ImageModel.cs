using Gallery.Server.Models.User;
using System.Text.Json.Serialization;

namespace Gallery.Server.Models.Files.Image
{
    public class ImageModel
    {
        public Guid ImageId { get; private set; }
        public string Name { get; set; }
        public string ImageFilePath { get; private set; }
        public string? Description { get; set; }
        public string ImageUrl { get; private set; }
        public DateTime CreateAt { get; private set; }
        public bool Publicity { get; set; }

        [JsonIgnore]
        public UserModel User { get; private set; }
        public Guid UserId { get; private set; }


        public ImageModel() { }

        private ImageModel(string name, string fileName, string? description, bool? publicity, UserModel user)
        {
            ImageId = Guid.NewGuid();
            Name = name;
            ImageFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "UsersData", user.UserId.ToString(), "Gallery", $"{ImageId}_{fileName}");
            Description = description ?? string.Empty;
            ImageUrl = $"/images/{user.UserId}/{ImageId}_{fileName}";
            CreateAt = DateTime.UtcNow;
            Publicity = publicity ?? true;
            User = user;
            UserId = user.UserId;
        }
        public static ImageModel Create(string name, string fileName, string? description, bool? publicity, UserModel user)
        {
            
            return new ImageModel(name, fileName, description, publicity, user);

        }
    }
}
