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
            {
                Directory.CreateDirectory(uploadPath);
            }

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
            return Task.Run(() =>
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            });
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public string GetFileUrl(string directory, string filePath, Guid userId)
        {
            var fileName = Path.GetFileName(filePath);
            return $"/images/{userId}/{directory}/{fileName}";
        }

        public string GetFilePath(string directory, Guid userId)
        {
            var basePath = Path.Combine(_environment.ContentRootPath, "Data", "UsersData", userId.ToString(), directory);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            return basePath;
        }
    }
}
