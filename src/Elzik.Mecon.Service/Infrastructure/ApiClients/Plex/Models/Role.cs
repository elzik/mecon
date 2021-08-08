using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models
{
    [XmlRoot(ElementName = "Role")]
    public class Role
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}