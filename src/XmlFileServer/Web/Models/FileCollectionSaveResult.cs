using System.Collections.ObjectModel;

namespace Web.Models
{
    public class ServiceResult<T>
    {
        private List<string> _validationErrors = new List<string>();

        public T Result { get; internal set; }

        public List<string> ValidationErrors => _validationErrors;

        internal void AddValidationError(string error)
        {
            _validationErrors.Add(error);
        }
    }
}
