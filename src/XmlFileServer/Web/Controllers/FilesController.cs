using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Web.Models;
using System.Dynamic;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;
using Web.Utilities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ISerializationUtility _serializationUtility;
        private readonly ILogger<FilesController> _logger;


        public FilesController(
            ISerializationUtility serializationUtility,
            ILogger<FilesController> logger)
        {
            _serializationUtility = serializationUtility;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ResponseModel<List<FileUploadResultModel>>> Upload([FromForm] FilesUploadModel model)
        {
            var fileProcessingTasks = model.Files
                .Select(async file =>
                {
                    using var readStream = file.OpenReadStream();
                    var json = await _serializationUtility.XmlToJsonAsync(readStream);
                    var fileName = file.FileName;
                });

            // process the files in parallel. As an alternative Task.Parallel library may be used
            await Task.WhenAll(fileProcessingTasks);

            return new ResponseModel<List<FileUploadResultModel>>(null);
        }

        [HttpGet(Name = "GetFiles")]
        public ResponseModel<List<FileInfoModel>> Get()
        {
            return new ResponseModel<List<FileInfoModel>>(new List<FileInfoModel> { new FileInfoModel() });
        }
    }
}