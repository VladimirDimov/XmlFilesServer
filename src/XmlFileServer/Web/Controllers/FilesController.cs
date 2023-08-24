using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<ResponseModel<FileSaveResult[]>> Upload([FromForm] FilesUploadModel model)
        {
            var fileProcessingTasks = model.Files
                .Select(async file =>
                {
                    using var readStream = file.OpenReadStream();
                    var json = await _serializationUtility.XmlToJsonAsync(readStream);
                    var fileName = file.FileName;

                    return await _fileUtility.SaveFileAsync(fileName, json, model.OverwriteExisting);
                });

            // process the files in parallel. As an alternative Task.Parallel library may be used
            var fileResults = await Task.WhenAll(fileProcessingTasks);

            return new ResponseModel<FileSaveResult[]>(fileResults);
        }

        [HttpGet(Name = "GetFiles")]
        public ResponseModel<List<FileInfoModel>> Get()
        {
            return new ResponseModel<List<FileInfoModel>>(new List<FileInfoModel> { new FileInfoModel() });
        }
    }
}