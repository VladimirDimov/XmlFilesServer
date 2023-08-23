using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetFiles")]
        public ResponseModel<List<FileInfoModel>> Get()
        {
            return new ResponseModel<List<FileInfoModel>>(new List<FileInfoModel> { new FileInfoModel() });
        }
    }
}