using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class GetFileModel
    {
        /// <summary>
        /// JSON file name
        /// </summary>
        [Required]
        public string FileName { get; set; }
    }
}
