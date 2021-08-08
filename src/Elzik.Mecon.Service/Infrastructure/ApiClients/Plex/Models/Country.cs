using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models
{
    [XmlRoot(ElementName = "Country")]
    public class Country
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}