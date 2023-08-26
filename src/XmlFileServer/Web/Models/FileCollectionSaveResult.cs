using System.Collections.ObjectModel;

namespace Web.Models
{
    public class FileCollectionSaveResult
    {
        private List<string> _validationErrors = new List<string>();

        public FileSaveResult[] Files { get; internal set; }

        public List<string> ValidationErrors => _validationErrors;

        internal void AddValidationError(string error)
        {
            _validationErrors.Add(error);
        }
    }
}
