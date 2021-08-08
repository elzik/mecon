using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models
{
    [XmlRoot(ElementName = "Director")]
    public class Director
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}