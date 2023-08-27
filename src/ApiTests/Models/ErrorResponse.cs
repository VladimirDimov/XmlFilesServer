using Newtonsoft.Json;

namespace ApiTests.Models
{

    public class ErrorResponseModel
    {
        public string Type { get; set; }
        
        public string Title { get; set; }
        
        public int Status { get; set; }
        
        public string TraceId { get; set; }
        
        public Errors Errors { get; set; }
    }

    public class Errors
    {
        public string[] Files { get; set; }

        [JsonProperty("Files[0]")]
        public string[] Files0 { get; set; }
    }
}
