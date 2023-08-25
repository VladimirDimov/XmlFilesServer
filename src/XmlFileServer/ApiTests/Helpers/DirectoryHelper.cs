namespace ApiTests.Helpers
{
    public class DirectoryHelper
    {
        private readonly TestAppSettings _settings;

        public DirectoryHelper(TestAppSettings settings)
        {
            _settings = settings;
        }

        public void EmptyJsonOutputDirectory()
        {
            foreach (var file in Directory.GetFiles(_settings.FileStoreLocation))
            {
                File.Delete(file);
            }
        }
    }
}
