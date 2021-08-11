using System.Xml.Serialization;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Role")]
    public class Role
    {
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }
}