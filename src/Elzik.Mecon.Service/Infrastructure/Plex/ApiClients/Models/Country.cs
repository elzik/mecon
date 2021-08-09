using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Country")]
    public class Country
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}