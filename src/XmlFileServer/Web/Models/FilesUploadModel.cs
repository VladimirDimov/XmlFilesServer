using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class FilesUploadModel
    {
        public bool OverwriteExisting { get; set; }

        [Required]
        public ICollection<IFormFile>? Files { get; set; }
    }
}
