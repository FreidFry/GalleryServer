using FluentValidation;
using Gallery.Server.Features.Profile.DTOs;
using Gallery.Server.Infrastructure.Persistence.db;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gallery.Server.Features.Profile.Validations
{
    public class ProfileAvatarValidator : AbstractValidator<UpdateProfileAvatar>
    {
        public ProfileAvatarValidator()
        {
            RuleFor(x => x.Avatar)
                .NotNull()
                .WithMessage("Image file is required.");

            RuleFor(x => x.Avatar)
                .Must(file => file != null && file.Length > 0)
                .WithMessage("Image file is required.");

            RuleFor(x => x.Avatar)
                .Must(file => file != null && file.Length <= 10 * 1024 * 1024)
                .WithMessage("Image file size must be less than 10MB.");

            RuleFor(x => x.Avatar)
                .Must(file => IsValidImageType(file))
                .WithMessage("Invalid image type. Allowed types are: .jpg, .jpeg, .png, .webp");

            RuleFor(x => x.UserId)
                .Must(id => IsValidUserId(id))
                .WithMessage("User id is empty");

        }

        bool IsValidImageType(IFormFile file)
        {
            if(file == null)
                return false;

            var validTypes = new[] { ".jpg", ".jpeg",".png", ".webp"};
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return validTypes.Contains(fileExtension);
        }

        bool IsValidUserId(Guid userId)
        {
            return userId != Guid.Empty;
        }
    }
}