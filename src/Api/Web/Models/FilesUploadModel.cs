using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class FilesUploadModel
    {
        [Required]
        [DefaultValue(true)]
        public bool OverwriteExisting { get; set; }

        [Required]
        public IEnumerable<IFormFile>? Files { get; set; }
    }
}
