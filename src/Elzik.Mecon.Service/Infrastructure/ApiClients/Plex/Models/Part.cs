using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models
{
    [XmlRoot(ElementName = "Part")]
    public class Part
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "duration")]
        public string Duration { get; set; }
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
        [XmlAttribute(AttributeName = "container")]
        public string Container { get; set; }
        [XmlAttribute(AttributeName = "indexes")]
        public string Indexes { get; set; }
        [XmlAttribute(AttributeName = "videoProfile")]
        public string VideoProfile { get; set; }
        [XmlAttribute(AttributeName = "audioProfile")]
        public string AudioProfile { get; set; }
        [XmlAttribute(AttributeName = "hasThumbnail")]
        public string HasThumbnail { get; set; }
        [XmlAttribute(AttributeName = "packetLength")]
        public string PacketLength { get; set; }
        [XmlAttribute(AttributeName = "has64bitOffsets")]
        public string Has64bitOffsets { get; set; }
        [XmlAttribute(AttributeName = "optimizedForStreaming")]
        public string OptimizedForStreaming { get; set; }
    }
}