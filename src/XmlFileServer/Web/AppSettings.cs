namespace Web
{
    public class AppSettings
    {
        public FileSettings FileSettings { get; set; }
    }

    public class FileSettings
    {
        public int MaxFileSizeInMegabytes { get; set; }
        public int MinFileNameLength { get; set; }
        public int MaxFileNameLength { get; set; }
    }
}
