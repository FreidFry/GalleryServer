using System.Text.Json.Serialization;

namespace Gallery.Server.Infrastructure.Persistence.Models
{
    public class ImageModel
    {
        public Guid ImageId { get; private set; }
        public string? Name { get; set; }
        public string ImageFilePath { get; private set; }
        public string? Description { get; set; }
        public string ImageUrl { get; private set; }
        public DateTime CreateAt { get; private set; } = DateTime.UtcNow;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public bool Publicity { get; set; }

        [JsonIgnore]
        public UserModel User { get; private set; }
        public Guid UserId { get; private set; }


        public ImageModel() { }

        ImageModel(string name, string fileName, string? description, bool? publicity, UserModel user)
        {
            ImageId = Guid.NewGuid();
            Name = name;
            ImageFilePath = GetImageFilePath(fileName);
            Description = description ?? string.Empty;
            ImageUrl = $"images/{user.UserId}/Gallery/{ImageId}_{fileName}";
            Publicity = publicity ?? true;
            User = user;
            UserId = user.UserId;
        }
        public static ImageModel Create(string name, string fileName, string? description, bool? publicity, UserModel user) =>
            new(name, fileName, description, publicity, user);

        string GetImageFilePath(string fileName) =>
            Path.Combine(Environment.CurrentDirectory,
                "Data",
                "UsersData",
                UserId.ToString(),
                "Gallery",
                $"{ImageId}_{fileName}"
            );
    }
}
