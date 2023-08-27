using Web.Models;

namespace Web.Services
{
    public interface IFileService
    {
        Task<ServiceResult<FileInfoModel>> GetFileAsync(string fileName);
        Task<ServiceResult<FileSaveResult[]>> SaveFilesAsync(FilesUploadModel model);
    }
}