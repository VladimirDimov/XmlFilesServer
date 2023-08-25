using ApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using Web.Models;

namespace ApiTests
{
    public class FileUploadSuccessTests
    {
        private readonly ApiClient _apiClient;
        private readonly FormFileHelper _formFileHelper;
        private readonly TestAppSettings _settings;
        private readonly TestFilesHelper _testFilesHelper;

        public FileUploadSuccessTests(
            ApiClient apiClient,
            FormFileHelper formFileHelper,
            TestAppSettings settings,
            TestFilesHelper testFilesHelper,
            DirectoryHelper directoryHelper)
        {
            _apiClient = apiClient;
            _formFileHelper = formFileHelper;
            _settings = settings;
            _testFilesHelper = testFilesHelper;

            directoryHelper.EmptyJsonOutputDirectory();
        }

        [Theory]
        [InlineData(true, "testFile1.xml")]
        [InlineData(false, "testFile1.xml")]
        [InlineData(true, "testFile1.a.b.c.xml")]
        [InlineData(false, "testFile1......xml")]
        public Task SingleFileUploadShouldReturnSuccessfulReponse(bool overwriteExisting, string fileName)
            => TestSuccessfulResponse(
                new List<TestFileContent>
                {
                    new TestFileContent
                    {
                        FileName = fileName,
                        Xml = "<a>123</a>"
                    }
                },
                overwriteExisting);

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public Task MultipleFilesUploadShouldReturnSuccessfulReponse(bool overwriteExisting)
            => TestSuccessfulResponse(
                new List<TestFileContent>
                {
                    new TestFileContent
                    {
                        FileName = "testFile1.xml",
                        Xml = "<a>1</a>"
                    },
                    new TestFileContent
                    {
                        FileName = "testFile2.xml",
                        Xml = "<a>2</a>"
                    },
                    new TestFileContent
                    {
                        FileName = "testFile3.xml",
                        Xml = "<a>3</a>"
                    },
                },
                overwriteExisting);

        [Fact]
        public async Task SingleFileUploadShouldStoreJsonWithExpectedContent()
        {
            var fileName = "mondial_1.7_MB.xml";
            var createdFileName = "mondial_1.7_MB.json";
            var xmlContent = await _testFilesHelper.ReadTestFileAsync(fileName);
            var expectedJsonContent = await _testFilesHelper.ReadTestFileAsync(createdFileName);

            var model = new FilesUploadModel
            {
                OverwriteExisting = true,
                Files = new List<IFormFile>
                {
                    _formFileHelper.FromXmlString(xmlContent, "Files", fileName)
                }
            };

            var response = await _apiClient.FilesPostAsync(model);

            Assert.True(response.IsSuccessStatusCode);

            var getFileResponse = await _apiClient.FilesGetAsync(createdFileName);
            var responseContent = await getFileResponse.Content.ReadAsStringAsync();

            Assert.Equal(expectedJsonContent, responseContent, ignoreWhiteSpaceDifferences: true);
        }

        private async Task TestSuccessfulResponse(IEnumerable<TestFileContent> files, bool overwriteExisting)
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = files.Select(f => _formFileHelper.FromXmlString(f.Xml, "Files", f.FileName))
            };

            var response = await _apiClient.FilesPostAsync(model);

            Assert.True(response.IsSuccessStatusCode);
        }

        public class TestFileContent
        {
            public string FileName { get; set; }

            public string Xml { get; set; }
        }
    }
}