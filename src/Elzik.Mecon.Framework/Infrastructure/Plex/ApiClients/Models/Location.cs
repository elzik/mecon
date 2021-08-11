using System.Xml.Serialization;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Location")]
    public class Location
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
    }
}