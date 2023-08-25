namespace Web
{
    public class AppSettings
    {
        public string FileStoreRelativePath { get; set; }

        public FileSettings FileSettings { get; set; }
    }

    public class FileSettings
    {
        public int MaxNumberOfFilesPerRequest { get; set; }
        public int MaxFileSizeInMegabytes { get; set; }
        public int MinFileNameLength { get; set; }
        public int MaxFileNameLength { get; set; }
    }
}
