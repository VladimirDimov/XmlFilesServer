using Newtonsoft.Json;
using System.Xml.Linq;

namespace Web.Utilities
{
    public class SerializationUtility : ISerializationUtility
    {
        private readonly XDeclaration _defaultDeclaration = new("1.0", null, null);

        public async Task<string> XmlToJsonAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var xmlContent = await reader.ReadToEndAsync();
            var json = XmlToJson(xmlContent);

            return json;
        }


        public string JsonToXml(string json)
        {
            var doc = JsonConvert.DeserializeXNode(json)!;
            var declaration = doc.Declaration ?? _defaultDeclaration;

            return $"{declaration}{Environment.NewLine}{doc}";
        }

        private string XmlToJson(string xml)
        {
            var doc = XDocument.Parse(xml);
            return JsonConvert.SerializeXNode(doc, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
