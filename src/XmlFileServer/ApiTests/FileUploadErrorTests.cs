using ApiTests.Helpers;
using ApiTests.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Web.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ApiTests
{
    public class FileUploadErrorTests
    {
        private readonly ApiClient _apiClient;
        private readonly FormFileHelper _formFileHelper;
        private readonly TestAppSettings _settings;
        private readonly TestFilesHelper _testFilesHelper;

        public FileUploadErrorTests(
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
        [InlineData(null, "<a>123</a>", "At least one file must be provided")]
        [InlineData("test_file_name.txt", "<a>123</a>", "File extension must be .xml")]
        [InlineData("test_file_name.xml", "", "Invalid empty file: test_file_name.xml")]
        [InlineData("test_file_name.xml", "{some invalid xml content}", "")]
        public Task SingleInvalidFileShouldReturnErrorReponse(string fileName, string xml, string errorMessage)
            => TestErrorResponse(
                new List<TestFileContent>
                {
                    new TestFileContent
                    {
                        FileName = fileName,
                        Xml = xml
                    }
                },
                true,
                errorMessage);

        [Fact]
        public async Task DuplicateFileNamesShouldReturnPropertError()
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = true,
                Files = new List<IFormFile>
                {
                    _formFileHelper.FromXmlString("<a>some valid content</a>", "Files", "file1.xml"),
                    _formFileHelper.FromXmlString("<a>some valid content</a>", "Files", "file1.xml"),
                }
            };

            var response = await _apiClient.FilesPostAsync(model);
            var content = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorResponseModel>(content);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.True(
                (errorModel.Errors.Files != null
                    && errorModel.Errors.Files.Any(err => err.Equals("Only unique file names are allowed within a single request. The following files appear more than once: file1.xml", StringComparison.OrdinalIgnoreCase))));
        }

        [Fact]
        public async Task TooManyFilesPerRequestShouldReturnPropertError()
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = true,
                Files = Enumerable.Range(1, 11).Select(i => _formFileHelper.FromXmlString("<a>some valid content</a>", "Files", $"file{i}.xml"))
            };

            var response = await _apiClient.FilesPostAsync(model);
            var content = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorResponseModel>(content);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.True(
                (errorModel.Errors.Files != null
                    && errorModel.Errors.Files.Any(err => err.Equals("The number of files excceeds the maximum allowed nomber of 10", StringComparison.OrdinalIgnoreCase))));
        }

        [Fact]
        public Task RequestWithNoFilesShouldReturnErrorReponse()
            => TestErrorResponse(
                new List<TestFileContent>(),
                true,
                "At least one file must be provided");

        private async Task TestErrorResponse(IEnumerable<TestFileContent> files, bool overwriteExisting, string errorMessage)
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = files.Select(f => _formFileHelper.FromXmlString(f.Xml, "Files", f.FileName))
            };

            var response = await _apiClient.FilesPostAsync(model);
            var content = await response.Content.ReadAsStringAsync();
            var errorModel = JsonConvert.DeserializeObject<ErrorResponseModel>(content);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            if (errorMessage != null)
            {
                Assert.True(
                    (errorModel.Errors.Files != null
                        && errorModel.Errors.Files.Any(err => err.Equals(errorMessage, StringComparison.OrdinalIgnoreCase)))
                    || (errorModel.Errors.Files0 != null
                            && errorModel.Errors.Files0.Any(err => err.Equals(errorMessage, StringComparison.OrdinalIgnoreCase))));
            }
        }

        public class TestFileContent
        {
            public string FileName { get; set; }

            public string Xml { get; set; }
        }
    }
}