using System.Reflection;

namespace ApiTests.Helpers
{
    public class TestFilesHelper
    {
        private string _rootFolderPath;

        public TestFilesHelper()
        {
            _rootFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../XmlFiles");
        }

        public async Task<string> ReadTestFileAsync(string fileName)
        {
            var filePath = Path.Combine(_rootFolderPath, fileName);

            return await File.ReadAllTextAsync(filePath);
        }
    }
}
