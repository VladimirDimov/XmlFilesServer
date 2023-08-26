using System.Xml;
using Web.Controllers;
using Web.Models;
using Web.Utilities;

namespace Web.Services
{
    public class FileService : IFileService
    {
        private readonly ISerializationUtility _serializationUtility;
        private readonly IFileUtility _fileUtility;
        private readonly ILogger<FilesController> _logger;

        public FileService(ISerializationUtility serializationUtility, IFileUtility fileUtility, ILogger<FilesController> logger)
        {
            _serializationUtility = serializationUtility;
            _fileUtility = fileUtility;
            _logger = logger;
        }

        public async Task<FileCollectionSaveResult> SaveFilesAsync(FilesUploadModel model)
        {
            var jsonFileContentTasks = model.Files
                .Select(async file =>
                {
                    using var readStream = file.OpenReadStream();

                    var fileName = file.FileName;

                    try
                    {
                        var json = await _serializationUtility.XmlToJsonAsync(readStream);

                        return new { FileName = file.FileName, Json = json, IsValidXml = true };
                    }
                    catch (XmlException)
                    {
                        return new { FileName = file.FileName, Json = "", IsValidXml = false };
                    }
                });

            var jsonFileContents = await Task.WhenAll(jsonFileContentTasks);

            var result = new FileCollectionSaveResult();

            var invalidFiles = jsonFileContents
                .Where(f => !f.IsValidXml)
                .Select(f => f.FileName)
                .ToList();

            if (invalidFiles.Any())
            {
                result.AddValidationError($"Invalid xml files: {string.Join(", ", invalidFiles)}");

                return result;
            }

            var fileProcessingTasks = jsonFileContents
                .Select(async file =>
                {
                    return await _fileUtility.SaveFileAsync(file.FileName, file.Json, model.OverwriteExisting);
                });

            // process the files in parallel. As an alternative approach Task.Parallel library may be used
            var fileResults = await Task.WhenAll(fileProcessingTasks);

            result.Files = fileResults;

            return result;
        }
    }
}
