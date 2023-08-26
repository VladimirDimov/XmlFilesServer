namespace Web.Models
{
    public class FileInfoModel
    {
        public FileInfoModel(byte[] bytes, string contentType, string fileName)
        {
            Bytes = bytes;
            ContentType = contentType;
            FileName = fileName;
        }

        public byte[] Bytes { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }
    }
}
