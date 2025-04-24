using System.Text.Json.Serialization;

namespace Gallery.Server.Infrastructure.Persistence.Models
{
    public class ImageModel
    {
        public Guid ImageId { get; private set; }
        public string? Name { get; set; }

        public string RealImagePath { get; set; }
        public string? TumbnailPath { get; set; }

        public string ImageUrl { get; set; }
        public string? TumbnailUrl { get; set; }

        public string? Description { get; set; }
        public bool Publicity { get; set; }

        public DateTime CreateAt { get; private set; } = DateTime.UtcNow;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

        [JsonIgnore] public UserModel User { get; private set; }
        public Guid UserId { get; private set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public ImageModel() { }

        ImageModel(string? name, string fileName, string? description, bool? publicity, UserModel user)
        {
            ImageId = Guid.NewGuid();
            Name = name ?? string.Empty;

            Description = description ?? string.Empty;
            Publicity = publicity ?? true;
            
            User = user;
            UserId = user.UserId;
        }
        public static ImageModel Create(string name, string fileName, string? description, bool? publicity, UserModel user) =>
            new(name, fileName, description, publicity, user);

    }
}
