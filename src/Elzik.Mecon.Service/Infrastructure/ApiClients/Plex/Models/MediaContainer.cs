using System.Collections.Generic;
using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.ApiClients.Plex.Models
{
    [XmlRoot(ElementName = "MediaContainer")]
	public class MediaContainer
	{
		[XmlElement(ElementName = "Video")]
		public List<Video> Video { get; set; }
		[XmlAttribute(AttributeName = "size")]
		public string Size { get; set; }
		[XmlAttribute(AttributeName = "allowSync")]
		public string AllowSync { get; set; }
		[XmlAttribute(AttributeName = "art")]
		public string Art { get; set; }
		[XmlAttribute(AttributeName = "identifier")]
		public string Identifier { get; set; }
		[XmlAttribute(AttributeName = "librarySectionID")]
		public string LibrarySectionID { get; set; }
		[XmlAttribute(AttributeName = "librarySectionTitle")]
		public string LibrarySectionTitle { get; set; }
		[XmlAttribute(AttributeName = "librarySectionUUID")]
		public string LibrarySectionUUID { get; set; }
		[XmlAttribute(AttributeName = "mediaTagPrefix")]
		public string MediaTagPrefix { get; set; }
		[XmlAttribute(AttributeName = "mediaTagVersion")]
		public string MediaTagVersion { get; set; }
		[XmlAttribute(AttributeName = "thumb")]
		public string Thumb { get; set; }
		[XmlAttribute(AttributeName = "title1")]
		public string Title1 { get; set; }
		[XmlAttribute(AttributeName = "title2")]
		public string Title2 { get; set; }
		[XmlAttribute(AttributeName = "viewGroup")]
		public string ViewGroup { get; set; }
		[XmlAttribute(AttributeName = "viewMode")]
		public string ViewMode { get; set; }
	}

}

