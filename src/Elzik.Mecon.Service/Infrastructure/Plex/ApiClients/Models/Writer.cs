using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Writer")]
    public class Writer
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}