using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Web.Models;
using Web.Utilities;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ISerializationUtility _serializationUtility;
        private readonly IFileUtility _fileUtility;
        private readonly ILogger<FilesController> _logger;


        public FilesController(
            ISerializationUtility serializationUtility,
            IFileUtility fileUtility,
            ILogger<FilesController> logger)
        {
            _serializationUtility = serializationUtility;
            _fileUtility = fileUtility;
            _logger = logger;
        }

        [HttpPost(Name = "Upload Files")]
        public async Task<IActionResult> UploadAsync([FromForm] FilesUploadModel model)
        {
            if (model?.Files is null)
                return BadRequest("Invalid null object");

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

            var invalidFiles = jsonFileContents
                .Where(f => !f.IsValidXml)
                .Select(f => f.FileName)
                .ToList();

            if (invalidFiles.Any())
            {
                return BadRequest($"Invalid xml files: {string.Join(", ", invalidFiles)}");
            }

            var fileProcessingTasks = jsonFileContents
                .Select(async file =>
                {
                    return await _fileUtility.SaveFileAsync(file.FileName, file.Json, model.OverwriteExisting);
                });

            // process the files in parallel. As an alternative approach Task.Parallel library may be used
            var fileResults = await Task.WhenAll(fileProcessingTasks);

            return Ok(fileResults);
        }

        [HttpGet(Name = "Get Files")]
        public async Task<IActionResult> GetAsync([FromQuery] GetFileModel model)
        {
            var fileByteArray = await _fileUtility.GetFileByteArray(model.FileName);

            if (fileByteArray is null)
            {
                return NotFound();
            }

            return File(fileByteArray, "text/json", model.FileName);
        }
    }
}