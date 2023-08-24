using System.Collections.Concurrent;
using System.Reflection;
using Web.Models;

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
            string filePath = Path.Combine(fileStoreDirectory, fileName);

            var semaphore = filesLock.GetOrAdd(filePath, new SemaphoreSlim(1, 1));

            var result = new FileSaveResult
            {
                FileName = fileName
            };

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
    }
}
