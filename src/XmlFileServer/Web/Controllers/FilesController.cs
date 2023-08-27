using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IFileService _fileService;


        public FilesController(
            IFileService fileService,
            ILogger<FilesController> logger)
        {
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Upload xml files
        /// </summary>
        /// <param name="model">Request model</param>
        /// <response code="200">Files stored successfully</response>
        /// <response code="400">Bad Request</response>
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

        /// <summary>
        /// Get JSON file
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [HttpGet(Name = "Get Files")]
        public async Task<IActionResult> GetAsync([FromQuery] GetFileModel model)
        {
            var result = await _fileService.GetFileAsync(model.FileName);

            if (result.ValidationErrors.Any())
                return BadRequest(result.ValidationErrors);

            if (result.Result.Bytes is null)
                return NotFound($"File {model.FileName} was not found.");

            return File(result.Result.Bytes, result.Result.ContentType, result.Result.FileName);
        }
    }
}