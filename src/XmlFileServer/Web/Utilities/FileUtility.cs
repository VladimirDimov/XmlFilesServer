using System.Collections.Concurrent;
using System.Reflection;

namespace Web.Utilities
{
    public class FileUtility : IFileUtility
    {
        private ConcurrentDictionary<string, SemaphoreSlim> filesLock = new ConcurrentDictionary<string, SemaphoreSlim>();

        public async Task SaveFileAsync(string fileName, string content, bool overwrite)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + fileName;

            var semaphore = filesLock.GetOrAdd(filePath, new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();

            try
            {
                if (!overwrite && File.Exists(fileName))
                    return;

                await File.WriteAllTextAsync(filePath, content);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
