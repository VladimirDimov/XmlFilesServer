using ApiTests.Extensions;
using ApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using Web.Models;

namespace ApiTests
{
    public class UnitTest1
    {
        private readonly ApiClient apiClient;
        private readonly FormFileHelper formFileHelper;

        public UnitTest1(ApiClient apiClient, FormFileHelper formFileHelper)
        {
            this.apiClient = apiClient;
            this.formFileHelper = formFileHelper;
        }

        [Fact]
        public async Task Test1()
        {
            var xml = "<a>123</a>";

            var model = new FilesUploadModel
            {
                OverwriteExisting = true,
                Files = new List<IFormFile>
                {
                    formFileHelper.FromXmlString(xml, "Files", "testFile1.xml"),
                }
            };
            var response = await apiClient.FilesPostAsync(model);
            var content = response.Content.ReadAsStringAsync().Result;
        }
    }
}