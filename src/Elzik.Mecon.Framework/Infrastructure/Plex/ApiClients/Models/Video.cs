using System.Collections.Generic;
using System.Xml.Serialization;

namespace Elzik.Mecon.Framework.Infrastructure.Plex.ApiClients.Models
{
    [XmlRoot(ElementName = "Video")]
    public class Video
    {
        [XmlElement(ElementName = "Media")]
        public List<Media> Media { get; set; }
        [XmlElement(ElementName = "Genre")]
        public List<Genre> Genre { get; set; }
        [XmlElement(ElementName = "Director")]
        public List<Director> Director { get; set; }
        [XmlElement(ElementName = "Writer")]
        public List<Writer> Writer { get; set; }
        [XmlElement(ElementName = "Country")]
        public List<Country> Country { get; set; }
        [XmlElement(ElementName = "Role")]
        public List<Role> Role { get; set; }
        [XmlAttribute(AttributeName = "ratingKey")]
        public string RatingKey { get; set; }
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "studio")]
        public string Studio { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "summary")]
        public string Summary { get; set; }
        [XmlAttribute(AttributeName = "rating")]
        public string Rating { get; set; }
        [XmlAttribute(AttributeName = "year")]
        public string Year { get; set; }
        [XmlAttribute(AttributeName = "thumb")]
        public string Thumb { get; set; }
        [XmlAttribute(AttributeName = "art")]
        public string Art { get; set; }
        [XmlAttribute(AttributeName = "duration")]
        public string Duration { get; set; }
        [XmlAttribute(AttributeName = "originallyAvailableAt")]
        public string OriginallyAvailableAt { get; set; }
        [XmlAttribute(AttributeName = "addedAt")]
        public string AddedAt { get; set; }
        [XmlAttribute(AttributeName = "updatedAt")]
        public string UpdatedAt { get; set; }
        [XmlAttribute(AttributeName = "chapterSource")]
        public string ChapterSource { get; set; }
        [XmlAttribute(AttributeName = "contentRating")]
        public string ContentRating { get; set; }
        [XmlAttribute(AttributeName = "primaryExtraKey")]
        public string PrimaryExtraKey { get; set; }
        [XmlAttribute(AttributeName = "tagline")]
        public string Tagline { get; set; }
        [XmlAttribute(AttributeName = "titleSort")]
        public string TitleSort { get; set; }
        [XmlAttribute(AttributeName = "viewCount")]
        public string ViewCount { get; set; }
        [XmlAttribute(AttributeName = "lastViewedAt")]
        public string LastViewedAt { get; set; }
        [XmlAttribute(AttributeName = "originalTitle")]
        public string OriginalTitle { get; set; }
        [XmlAttribute(AttributeName = "skipCount")]
        public string SkipCount { get; set; }
        [XmlAttribute(AttributeName = "viewOffset")]
        public string ViewOffset { get; set; }
        [XmlAttribute(AttributeName = "userRating")]
        public string UserRating { get; set; }
        [XmlAttribute(AttributeName = "lastRatedAt")]
        public string LastRatedAt { get; set; }
    }
}