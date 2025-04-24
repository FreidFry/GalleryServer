using FluentValidation;
using FluentValidation.AspNetCore;
using Gallery.Server.Core.Extencions.LoadModules;
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
using Gallery.Server.Infrastructure.Persistence.db;
using Gallery.Server.Infrastructure.Persistence.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using static Gallery.Server.Core.Extencions.LoadModules.JwtAutheticationExtencions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = ["text/plain", "text/css", "application/javascript", "application/json", "image/svg+xml"];
});

builder.Services
    .AddJwtAuthentication()
    .AddPortConfiguration(builder.WebHost);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll",
        builder =>
            builder.WithOrigins("https://localhost:24815", "http://localhost:24815")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()
    )
);
builder.Services.AddLogging(builder =>
    builder.AddConsole()
    .AddDebug()
);

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddScoped<IValidator<ImageUploadDto>, ImageUploadValidator>();
builder.Services.AddScoped<IValidator<UpdateProfileAvatar>, ProfileAvatarValidator>();
builder.Services.AddScoped<IHttpContextHelper, HttpContextHelper>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Data", "default")),
    RequestPath = "/default",
    ServeUnknownFileTypes = true
});

#pragma warning disable CS8604 // Possible null reference argument.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), app.Configuration.GetValue<string>("FileStorage:UserDataBasePath"))),
    RequestPath = "/images",
    ServeUnknownFileTypes = true
});
#pragma warning restore CS8604 // Possible null reference argument.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
