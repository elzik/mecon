using System.Collections.Generic;
using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.Plex.ApiClients.Models
{
    public class Library
    {
        [XmlElement(ElementName = "Location")]
        public List<Location> Location { get; set; }
        [XmlAttribute(AttributeName = "allowSync")]
        public string AllowSync { get; set; }
        [XmlAttribute(AttributeName = "art")]
        public string Art { get; set; }
        [XmlAttribute(AttributeName = "composite")]
        public string Composite { get; set; }
        [XmlAttribute(AttributeName = "filters")]
        public string Filters { get; set; }
        [XmlAttribute(AttributeName = "refreshing")]
        public string Refreshing { get; set; }
        [XmlAttribute(AttributeName = "thumb")]
        public string Thumb { get; set; }
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "agent")]
        public string Agent { get; set; }
        [XmlAttribute(AttributeName = "scanner")]
        public string Scanner { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "uuid")]
        public string Uuid { get; set; }
        [XmlAttribute(AttributeName = "updatedAt")]
        public string UpdatedAt { get; set; }
        [XmlAttribute(AttributeName = "createdAt")]
        public string CreatedAt { get; set; }
        [XmlAttribute(AttributeName = "scannedAt")]
        public string ScannedAt { get; set; }
        [XmlAttribute(AttributeName = "content")]
        public string Content { get; set; }
        [XmlAttribute(AttributeName = "directory")]
        public string Directory { get; set; }
        [XmlAttribute(AttributeName = "contentChangedAt")]
        public string ContentChangedAt { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
        [XmlAttribute(AttributeName = "enableAutoPhotoTags")]
        public string EnableAutoPhotoTags { get; set; }
    }
}