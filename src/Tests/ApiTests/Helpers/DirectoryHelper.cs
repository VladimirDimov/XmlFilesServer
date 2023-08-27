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
            if (!Directory.Exists(_settings.FileStoreLocation))
                return;

            var retry = 0;

            while (retry < 3)
            {
                try
                {
                    foreach (var file in Directory.GetFiles(_settings.FileStoreLocation))
                    {
                        File.Delete(file);
                    }

                    return;
                }
                catch
                {
                    Thread.Sleep(200 * retry);
                }
            }
        }
    }
}
