using ApiTests.Extensions;
using System.Net.Http.Headers;
using Web.Models;

namespace ApiTests
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("XmlFileServer");
        }

        public async Task<HttpResponseMessage> FilesPostAsync(FilesUploadModel model)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.OverwriteExisting.ToString()), nameof(FilesUploadModel.OverwriteExisting));

            foreach (var file in model.Files)
            {
                var fileContent = CreateFileContent(file.OpenReadStream(), file.Name, file.FileName, "text/xml");

                content.Add(fileContent);
            }


            return await _httpClient.PostAsync("Files", content);
        }

        public async Task<HttpResponseMessage> FilesGetAsync(string fileName)
        {
            string uri = $"Files?fileName={fileName}";

            return await _httpClient.GetAsync(uri);
        }

        private StreamContent CreateFileContent(Stream stream, string name, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"" + name + "\"",
                FileName = "\"" + fileName + "\""
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }
    }
}
