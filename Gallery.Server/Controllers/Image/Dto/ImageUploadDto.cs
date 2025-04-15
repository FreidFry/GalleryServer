using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Gallery.Server.Controllers.Image.Dto
{
    public class ImageUploadDto
    {
        [Required]
        public IFormFile Image { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool? Publicity { get; set; }
    }
}
