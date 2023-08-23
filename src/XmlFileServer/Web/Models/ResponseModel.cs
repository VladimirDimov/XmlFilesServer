namespace Web.Models
{
    public class ResponseModel<T>
    {
        public ResponseModel(T content)
        {
            Content = content;
        }

        public bool IsSuccess { get; set; }

        public T Content { get; set; }

        public List<string> ValidationErrors { get; set; }
    }
}
