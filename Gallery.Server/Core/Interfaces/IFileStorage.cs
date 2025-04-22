namespace Gallery.Server.Core.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(IFormFile file, string directory, string fileName);
        Task DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
        string GetFileUrl(string directory, string filePath, Guid userId);
        string GetFilePath(string directory, Guid userId);
    }
}
