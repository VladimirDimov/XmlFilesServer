using Microsoft.Extensions.Logging;
using Web.Controllers;
using Web.Services;
using Web.Utilities;

namespace UnitTests
{
    public class FilesControllerTests
    {
        private readonly FilesController _controller;
        private ISerializationUtility _serializationUtilityMock;
        private IFileUtility _fileUtilityMock;
        private ILogger<FilesController> _loggerMock;
        private IFileService _fileServiceMock;

        public FilesControllerTests()
        {

            _controller = new FilesController(_serializationUtilityMock, _fileUtilityMock, _loggerMock, _fileServiceMock);
        }

        [Fact]
        public void Test1()
        {

        }
    }
}