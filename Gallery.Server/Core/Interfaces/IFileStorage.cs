namespace Gallery.Server.Core.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(IFormFile file, string directory, string fileName);
        Task DeleteFileAsync(string filePath);
        bool FileExists(string filePath);
        string GetFilePath(string filePath);
    }
}
