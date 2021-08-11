using System.Xml.Serialization;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Writer")]
    public class Writer
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}