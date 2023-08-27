using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using Web.Models;

using static Web.Constants;

namespace Web.Utilities
{
    public class FileUtility : IFileUtility
    {
        private ConcurrentDictionary<string, SemaphoreSlim> filesLock = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly string fileStoreDirectory;

        public FileUtility(AppSettings appSettings)
        {
            var rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            fileStoreDirectory = Path.Combine(rootDirectory, appSettings.FileStoreRelativePath);

            if (!Directory.Exists(fileStoreDirectory))
            {
                Directory.CreateDirectory(fileStoreDirectory);
            }
        }

        public async Task<FileSaveResult> SaveFileAsync(string fileName, string content, bool overwrite)
        {
            var jsonFileName = FileNameToJson(fileName);
            var filePath = Path.Combine(fileStoreDirectory, jsonFileName);

            var result = new FileSaveResult
            {
                FileName = jsonFileName
            };

            var semaphore = filesLock.GetOrAdd(filePath, new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();

            try
            {
                bool isExistingFile = File.Exists(filePath);

                if (!overwrite && isExistingFile)
                {
                    result.IsSuccess = true;
                    result.Message = "File already existing.";

                    return result;
                }

                await File.WriteAllTextAsync(filePath, content);

                result.IsSuccess = true;
                result.Message = isExistingFile
                                    ? "File successfully overwritten"
                                    : "File successfully stored";

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error = "Unexpected error occured";

                return result;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<byte[]?> GetFileByteArrayAsync(string fileName)
        {
            var filePath = Path.Combine(fileStoreDirectory, fileName);

            var semaphore = filesLock.GetOrAdd(filePath, new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                if (!File.Exists(filePath))
                    return null;

                return await File.ReadAllBytesAsync(filePath);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private string FileNameToJson(string fileName)
        {
            var lastDotIndex = fileName.LastIndexOf('.');
            var fileNameBuilder = new StringBuilder(fileName);
            fileNameBuilder.Remove(lastDotIndex, fileName.Length - lastDotIndex);
            fileNameBuilder.Append(CommonConstants.JSON_FILE_EXTENSION);

            return fileNameBuilder.ToString();
        }
    }
}
