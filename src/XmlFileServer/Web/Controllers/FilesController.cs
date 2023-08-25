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

            var fileProcessingTasks = model.Files
                .Select(async file =>
                {
                    using var readStream = file.OpenReadStream();

                    var fileName = file.FileName;

                    try
                    {
                        var json = await _serializationUtility.XmlToJsonAsync(readStream);

                        return await _fileUtility.SaveFileAsync(fileName, json, model.OverwriteExisting);
                    }
                    catch (XmlException)
                    {
                        return new FileSaveResult { IsSuccess = false, FileName = file.FileName, Error = "Invalid xml content" };
                    }

                });

            // process the files in parallel. As an alternative Task.Parallel library may be used
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