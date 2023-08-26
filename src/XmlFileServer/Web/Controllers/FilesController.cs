using Microsoft.AspNetCore.Mvc;
using System.Reflection;
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
                return BadRequest(fileSaveResult.ValidationErrors);

            return Ok(fileSaveResult);
        }

        [HttpGet(Name = "Get Files")]
        public async Task<IActionResult> GetAsync([FromQuery] GetFileModel model)
        {
            var result = await _fileService.GetFileAsync(model.FileName);

            if (result.ValidationErrors.Any())
                return BadRequest(result.ValidationErrors);

            return File(result.Result.Bytes, result.Result.ContentType, result.Result.FileName);
        }
    }
}