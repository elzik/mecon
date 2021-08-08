using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models
{
    [XmlRoot(ElementName = "Genre")]
    public class Genre
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}