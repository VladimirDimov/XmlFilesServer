using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Web.Models;
using Web.Services;
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
        private readonly IFileService _fileService;


        public FilesController(
            ISerializationUtility serializationUtility,
            IFileUtility fileUtility,
            ILogger<FilesController> logger,
            IFileService fileService)
        {
            _serializationUtility = serializationUtility;
            _fileUtility = fileUtility;
            _logger = logger;
            _fileService = fileService;
        }

        [HttpPost(Name = "Upload Files")]
        public async Task<IActionResult> UploadAsync([FromForm] FilesUploadModel model)
        {
            if (model?.Files is null)
                return BadRequest("Invalid null object");

            var fileSaveResult = await _fileService.SaveFilesAsync(model);

            if (fileSaveResult.ValidationErrors.Any())
                return BadRequest(fileSaveResult.ValidationErrors.First());

            return Ok(fileSaveResult);
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