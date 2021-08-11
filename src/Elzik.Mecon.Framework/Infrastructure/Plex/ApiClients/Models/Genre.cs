using System.Xml.Serialization;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Genre")]
    public class Genre
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}