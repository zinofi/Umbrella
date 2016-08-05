using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Umbrella.WebUtilities.SiteMap
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class XmlSiteMap
    {
        public XmlSiteMap()
        {
            Urls = new List<XmlSiteMapUrl>();
        }

        [XmlElement("url")]
        public List<XmlSiteMapUrl> Urls { get; set; }

        public XmlSiteMap Add(string url, ChangeFrequency? changeFrequency = null, DateTime? lastModified = null, double? priority = null)
        {
            url = url.ToLower();
            
            //Ensure the list doesn't already include this URL
            if (!Urls.Any(x => x.Url == url))
            {
                Urls.Add(new XmlSiteMapUrl
                {
                    Url = url,
                    ChangeFrequency = changeFrequency,
                    LastModified = lastModified.HasValue ? lastModified.Value.ToUniversalTime().ToString("o") : "",
                    Priority = priority
                });
            }

            return this;
        }

        public override string ToString()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(XmlSiteMap));
                xs.Serialize(ms, this);

                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}