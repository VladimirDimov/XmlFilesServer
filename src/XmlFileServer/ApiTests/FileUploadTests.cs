using ApiTests.Extensions;
using ApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using Web.Models;

namespace ApiTests
{
    public class FileUploadTests
    {
        private readonly ApiClient apiClient;
        private readonly FormFileHelper formFileHelper;

        public FileUploadTests(ApiClient apiClient, FormFileHelper formFileHelper)
        {
            this.apiClient = apiClient;
            this.formFileHelper = formFileHelper;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public Task SingleFileUploadShouldReturnSuccessfulReponse(bool overwriteExisting)
            => TestSuccessfulResponse(
                new List<TestFileContent>
                {
                    new TestFileContent
                    {
                        FileName = "testFile1.xml",
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public Task MultipleFilesWithRepeatedFileNameShouldReturnSuccessfulReponse(bool overwriteExisting)
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
                        FileName = "testFile1.xml",
                        Xml = "<a>2</a>"
                    },
                    new TestFileContent
                    {
                        FileName = "testFile1.xml",
                        Xml = "<a>3</a>"
                    },
                },
                overwriteExisting);

        private async Task TestSuccessfulResponse(IEnumerable<TestFileContent> files, bool overwriteExisting)
        {
            var model = new FilesUploadModel
            {
                OverwriteExisting = overwriteExisting,
                Files = files.Select(f => formFileHelper.FromXmlString(f.Xml, "Files", f.FileName))
            };

            var response = await apiClient.FilesPostAsync(model);

            Assert.True(response.IsSuccessStatusCode);
        }

        public class TestFileContent
        {
            public string FileName { get; set; }

            public string Xml { get; set; }
        }
    }
}