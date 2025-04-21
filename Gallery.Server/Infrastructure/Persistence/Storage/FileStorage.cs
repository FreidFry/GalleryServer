using Gallery.Server.Core.Interfaces;

namespace Gallery.Server.Infrastructure.Persistence.Storage
{
    public class FileStorage : IFileStorage
    {
        public IConfiguration _configuration { get; }
        public IWebHostEnvironment _environment { get; }

        public FileStorage(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string directory, string fileName)
        {
            var uploadPath = Path.Combine(_environment.ContentRootPath, directory);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            return Task.CompletedTask;
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetFilePath(string filePath)
        {
            var relativeFilePath = Path.GetRelativePath(_environment.ContentRootPath, filePath);
            return $"{relativeFilePath.Replace("\\", "/")}";
        }

    }
}
