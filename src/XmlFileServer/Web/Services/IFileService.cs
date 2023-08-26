using Web.Models;

namespace Web.Services
{
    public interface IFileService
    {
        Task<FileCollectionSaveResult> SaveFilesAsync(FilesUploadModel model);
    }
}