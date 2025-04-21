using FluentValidation;
using Gallery.Server.Features.Image.DTOs;

namespace Gallery.Server.Features.Image.Validations
{
    public class ImageUploadValidator : AbstractValidator<ImageUploadDto>
    {
        public ImageUploadValidator()
        {
            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("Image file is required.");

            RuleFor(x => x.Image)
                .Must(file => file != null && file.Length > 0)
                .WithMessage("Image file is required.");

            RuleFor(x => x.Image)
                .Must(file => file != null && file.Length <= 10 * 1024 * 1024)
                .WithMessage("Image file size must be less than 10MB.");

            RuleFor(x => x.Image)
                .Must(file => IsValidImageType(file))
                .WithMessage("Invalid image type. Allowed types are: .jpg, .jpeg, .png, .webp, .gif.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Image name is required.")
                .MaximumLength(100)
                .WithMessage("Image name must be less than 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Image description must be less than 500 characters.");

        }

        bool IsValidImageType(IFormFile file)
        {
            if(file == null)
                return false;

            var validTypes = new[] { ".jpg", ".jpeg",".png", ".webp", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return validTypes.Contains(fileExtension);
        }
    }
}
