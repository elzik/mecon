using System.Xml.Serialization;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Director")]
    public class Director
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}