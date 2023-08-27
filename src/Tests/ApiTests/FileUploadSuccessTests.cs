using ApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Web.Models;

namespace ApiTests
{
    public class FileUploadSuccessTests
    {
        private readonly ApiClient _apiClient;
        private readonly FormFileHelper _formFileHelper;
        private readonly TestFilesHelper _testFilesHelper;

        public FileUploadSuccessTests(
            ApiClient apiClient,
            FormFileHelper formFileHelper,
            TestFilesHelper testFilesHelper,
            DirectoryHelper directoryHelper)
        {
            _apiClient = apiClient;
            _formFileHelper = formFileHelper;
            _testFilesHelper = testFilesHelper;

            directoryHelper.EmptyJsonOutputDirectory();
        }

        [Theory]
        [InlineData(true, "testFile1.xml")]
        [InlineData(false, "testFile1.xml")]
        [InlineData(true, "testFile1.a.b.c.xml")]
        [InlineData(false, "testFile1......xml")]
        public Task SingleFileUploadShouldReturnSuccessfulReponse(bool overwriteExisting, string fileName)
            => AssertSuccessfulResponseAsync(
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
            => AssertSuccessfulResponseAsync(
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

        [Fact]
        public async Task MaxAllowedNumberOfFilesShouldWorkAsExpected()
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = true,
                Files = Enumerable.Range(1, 10).Select(i => _formFileHelper.FromXmlString("<a>some valid content</a>", "Files", $"file{i}.xml"))
            };

            var response = await _apiClient.FilesPostAsync(model);
            var content = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task AssertSuccessfulResponseAsync(IEnumerable<TestFileContent> files, bool overwriteExisting)
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = files.Select(f => _formFileHelper.FromXmlString(f.Xml, "Files", f.FileName))
            };

            var response = await _apiClient.FilesPostAsync(model);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ShouldOverwriteExistingFileWhenOverwriteIsSetToTrue()
        {
            const string xmlFile1ContentVersion1 = "<tag1>some test content 1 version 1</tag1>";
            const string xmlFile2ContentVersion1 = "<tag1>some test content 2 version 1</tag1>";
            const string xmlFile3ContentVersion1 = "<tag1>some test content 3 version 1</tag1>";

            const string jsonFile1ContentVersion1 = "{\"tag1\": \"some test content 1 version 1\"}";
            const string jsonFile2ContentVersion1 = "{\"tag1\": \"some test content 2 version 1\"}";
            const string jsonFile3ContentVersion1 = "{\"tag1\": \"some test content 3 version 1\"}";

            const string xmlFile1ContentVersion2 = "<tag1>some test content 1 version 2</tag1>";
            const string xmlFile2ContentVersion2 = "<tag1>some test content 2 version 2</tag1>";
            const string xmlFile3ContentVersion2 = "<tag1>some test content 3 version 2</tag1>";

            const string jsonFile1ContentVersion2 = "{\"tag1\": \"some test content 1 version 2\"}";
            const string jsonFile2ContentVersion2 = "{\"tag1\": \"some test content 2 version 2\"}";
            const string jsonFile3ContentVersion2 = "{\"tag1\": \"some test content 3 version 2\"}";


            const string fileName1 = "file_1.xml";
            const string fileName2 = "file_2.xml";
            const string fileName3 = "file_3.xml";

            var filesVersion1 = new List<TestFileContent>
            {
                new TestFileContent { FileName = fileName1, Xml = xmlFile1ContentVersion1 },
                new TestFileContent { FileName = fileName2, Xml = xmlFile2ContentVersion1 },
                new TestFileContent { FileName = fileName3, Xml = xmlFile3ContentVersion1 },
            };

            await AssertJsonContentAsync(
                   filesVersion1,
                   new[] { jsonFile1ContentVersion1, jsonFile2ContentVersion1, jsonFile3ContentVersion1 },
                   true);

            var filesVersion2 = new List<TestFileContent>
            {
                new TestFileContent { FileName = fileName1, Xml = xmlFile1ContentVersion2 },
                new TestFileContent { FileName = fileName2, Xml = xmlFile2ContentVersion2 },
                new TestFileContent { FileName = fileName3, Xml = xmlFile3ContentVersion2 },
            };

            await AssertJsonContentAsync(
                   filesVersion2,
                   new[] { jsonFile1ContentVersion2, jsonFile2ContentVersion2, jsonFile3ContentVersion2 },
                   true);
        }

        [Fact]
        public async Task ShouldNotOverwriteExistingFileWhenOverwriteIsSetToFalse()
        {
            const string xmlFile1ContentVersion1 = "<tag1>some test content 1 version 1</tag1>";
            const string xmlFile2ContentVersion1 = "<tag1>some test content 2 version 1</tag1>";
            const string xmlFile3ContentVersion1 = "<tag1>some test content 3 version 1</tag1>";

            const string jsonFile1ContentVersion1 = "{\"tag1\": \"some test content 1 version 1\"}";
            const string jsonFile2ContentVersion1 = "{\"tag1\": \"some test content 2 version 1\"}";
            const string jsonFile3ContentVersion1 = "{\"tag1\": \"some test content 3 version 1\"}";

            const string xmlFile1ContentVersion2 = "<tag1>some test content 1 version 2</tag1>";
            const string xmlFile2ContentVersion2 = "<tag1>some test content 2 version 2</tag1>";
            const string xmlFile3ContentVersion2 = "<tag1>some test content 3 version 2</tag1>";

            const string fileName1 = "file_1.xml";
            const string fileName2 = "file_2.xml";
            const string fileName3 = "file_3.xml";

            var filesVersion1 = new List<TestFileContent>
            {
                new TestFileContent { FileName = fileName1, Xml = xmlFile1ContentVersion1 },
                new TestFileContent { FileName = fileName2, Xml = xmlFile2ContentVersion1 },
                new TestFileContent { FileName = fileName3, Xml = xmlFile3ContentVersion1 },
            };

            await AssertJsonContentAsync(
                   filesVersion1,
                   new[] { jsonFile1ContentVersion1, jsonFile2ContentVersion1, jsonFile3ContentVersion1 },
                   true);

            var filesVersion2 = new List<TestFileContent>
            {
                new TestFileContent { FileName = fileName1, Xml = xmlFile1ContentVersion2 },
                new TestFileContent { FileName = fileName2, Xml = xmlFile2ContentVersion2 },
                new TestFileContent { FileName = fileName3, Xml = xmlFile3ContentVersion2 },
            };

            await AssertJsonContentAsync(
                   filesVersion2,
                   new[] { jsonFile1ContentVersion1, jsonFile2ContentVersion1, jsonFile3ContentVersion1 },
                   false);
        }

        [Fact]
        public async Task ShouldWorkProperlyIfMultipleParallelRequestsAreUploadingTheSameFileName()
        {
            var fileName = "file1.xml";

            var fileUploadTasks = Enumerable.Range(1, 20).Select(_ => AssertSuccessfulResponseAsync(
                 new List<TestFileContent>
                 {
                    new TestFileContent
                    {
                        FileName = fileName,
                        Xml = "<a>123</a>"
                    }
                 },
                 true));

            await Task.WhenAll(fileUploadTasks);
        }

        private async Task AssertJsonContentAsync(IEnumerable<TestFileContent> files, IEnumerable<string> expectedJsonContent, bool overwriteExisting)
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = files.Select(f => _formFileHelper.FromXmlString(f.Xml, "Files", f.FileName))
            };

            var response = await _apiClient.FilesPostAsync(model);

            Assert.True(response.IsSuccessStatusCode);

            var storedFilesTask = files
                .Select(f =>
                {
                    var jsonFileName = Path.GetFileNameWithoutExtension(f.FileName) + ".json";
                    return _apiClient.FilesGetAsync(jsonFileName);
                })
                .ToArray();

            var storedFileResponsMessages = await Task.WhenAll(storedFilesTask);

            var assertFiles = expectedJsonContent.Zip(storedFileResponsMessages);

            // Assert that number of stored files is equal to the number of input files
            Assert.Equal(files.Count(), assertFiles.Count());

            foreach (var assertFile in assertFiles)
            {
                var expectedObject = JsonConvert.DeserializeObject(assertFile.First);
                var actualObject = JsonConvert.DeserializeObject(assertFile.Second.Content.ReadAsStringAsync().Result);

                Assert.Equal(expectedObject, actualObject);
            }
        }

        public class TestFileContent
        {
            public string FileName { get; set; }

            public string Xml { get; set; }
        }
    }
}