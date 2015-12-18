using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Umbrella.WebUtilities.SiteMap
{
    public class XmlSiteMapUrl
    {
        [XmlElement("changefreq")]
        public ChangeFrequency? ChangeFrequency { get; set; }

        [XmlElement("lastmod")]
        public string LastModified { get; set; }

        [XmlElement("priority")]
        public double? Priority { get; set; }

        [XmlElement("loc")]
        public string Url { get; set; }

        public bool ShouldSerializeChangeFrequency()
        {
            return ChangeFrequency.HasValue;
        }

        public bool ShouldSerializeLastModified()
        {
            return !string.IsNullOrEmpty(LastModified);
        }

        public bool ShouldSerializePriority()
        {
            return Priority.HasValue;
        }
    }
}