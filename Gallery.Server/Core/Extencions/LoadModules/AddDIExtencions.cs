using FluentValidation.AspNetCore;
using FluentValidation;
using Gallery.Server.Core.Helpers;
using Gallery.Server.Core.Interfaces;
using Gallery.Server.Core.Services;
using Gallery.Server.Features.Image.DTOs;
using Gallery.Server.Features.Image.Services;
using Gallery.Server.Features.Image.Validations;
using Gallery.Server.Features.Profile.DTOs;
using Gallery.Server.Features.Profile.Services;
using Gallery.Server.Features.Profile.Validations;
using Gallery.Server.Features.User.Services;
using Gallery.Server.Infrastructure.Persistence.Storage;

namespace Gallery.Server.Core.Extencions.LoadModules
{
    internal static class AddDIExtencions
    {
        internal static IServiceCollection AddDipencyInjections(this IServiceCollection services)
        {
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IFileStorage, FileStorage>();
            services.AddScoped<IValidator<ImageUploadDto>, ImageUploadValidator>();
            services.AddScoped<IValidator<UpdateProfileAvatar>, ProfileAvatarValidator>();
            services.AddScoped<IHttpContextHelper, HttpContextHelper>();
            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}
