using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class FilesUploadModel
    {
        public bool OverwriteExisting { get; set; } = false;

        public IEnumerable<IFormFile>? Files { get; set; }
    }
}
