using System.ComponentModel.DataAnnotations;

namespace Gallery.Server.Features.Image.DTOs
{
    public class ImageUploadDto
    {
        [Required] public IFormFile Image { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? Publicity { get; set; }
    }
}
