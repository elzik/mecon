using System.Collections.Generic;
using System.Xml.Serialization;

namespace Elzik.Mecon.Service.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "MediaContainer")]
	public class LibraryContainer
	{
		[XmlElement(ElementName = "Directory")]
		public List<Library> Directory { get; set; }
		[XmlAttribute(AttributeName = "size")]
		public string Size { get; set; }
		[XmlAttribute(AttributeName = "allowSync")]
		public string AllowSync { get; set; }
		[XmlAttribute(AttributeName = "identifier")]
		public string Identifier { get; set; }
		[XmlAttribute(AttributeName = "mediaTagPrefix")]
		public string MediaTagPrefix { get; set; }
		[XmlAttribute(AttributeName = "mediaTagVersion")]
		public string MediaTagVersion { get; set; }
		[XmlAttribute(AttributeName = "title")]
		public string Title { get; set; }
	}

}
