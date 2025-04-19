namespace Gallery.Server.Features.Image.DTOs
{
    public class ImageUpdateDto
    {
        public Guid ImageId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Publicity { get; set; }
    }
}
