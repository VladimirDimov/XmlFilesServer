namespace Web.Utilities
{
    public interface ISerializationUtility
    {
        string JsonToXml(string json);
        Task<string> XmlToJsonAsync(Stream stream);
    }
}