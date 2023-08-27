namespace Web.Models
{
    public class FileSaveResult
    {
        public string FileName { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string Error { get; set; }
    }
}
