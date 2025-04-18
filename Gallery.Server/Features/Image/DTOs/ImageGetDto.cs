using Gallery.Server.Infrastructure.Persistence.Models;

namespace Gallery.Server.Features.Image.DTOs
{
    public class ImageGetDto
    {
        public Guid ImageId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }

        public static ImageGetDto FromModel(ImageModel imageModel)
        {
            return new ImageGetDto
            {
                ImageId = imageModel.ImageId,
                Name = imageModel.Name,
                Description = imageModel.Description,
                ImageUrl = imageModel.ImageUrl,
                CreateAt = imageModel.CreateAt
            };
        }
    }
}
