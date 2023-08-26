using Web.Models;

namespace Web.Utilities
{
    public interface IFileUtility
    {
        Task<FileSaveResult> SaveFileAsync(string fileName, string content, bool overwrite);
        Task<byte[]?> GetFileByteArrayAsync(string fileName);
    }
}