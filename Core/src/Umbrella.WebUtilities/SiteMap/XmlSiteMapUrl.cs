using System.Xml.Serialization;

namespace Umbrella.WebUtilities.SiteMap
{
	/// <summary>
	/// Represents an entry in an XML SiteMap.
	/// </summary>
	public class XmlSiteMapUrl
	{
		/// <summary>
		/// Gets or sets the change frequency.
		/// </summary>
		[XmlElement("changefreq")]
		public ChangeFrequency? ChangeFrequency { get; set; }

		/// <summary>
		/// Gets or sets the last modified date
		/// </summary>
		[XmlElement("lastmod")]
		public string LastModified { get; set; }

		/// <summary>
		/// Gets or sets the priority.
		/// </summary>
		[XmlElement("priority")]
		public double? Priority { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		[XmlElement("loc")]
		public string Url { get; set; }

		/// <summary>
		/// Determines if the <see cref="ChangeFrequency"/> should be included when this instance is serialized.
		/// This method follows the naming convention for methods picked up at runtime by the <see cref="XmlSerializer"/>: ShouldSerialize{PropertyName}
		/// </summary>
		/// <returns><see langword="true" /> if yes; otherwise <see langword="false" />.</returns>
		public bool ShouldSerializeChangeFrequency() => ChangeFrequency.HasValue;

		/// <summary>
		/// Determines if the <see cref="LastModified"/> should be included when this instance is serialized.
		/// This method follows the naming convention for methods picked up at runtime by the <see cref="XmlSerializer"/>: ShouldSerialize{PropertyName}
		/// </summary>
		/// <returns><see langword="true" /> if yes; otherwise <see langword="false" />.</returns>
		public bool ShouldSerializeLastModified() => !string.IsNullOrEmpty(LastModified);

		/// <summary>
		/// Determines if the <see cref="Priority"/> should be included when this instance is serialized.
		/// This method follows the naming convention for methods picked up at runtime by the <see cref="XmlSerializer"/>: ShouldSerialize{PropertyName}
		/// </summary>
		/// <returns><see langword="true" /> if yes; otherwise <see langword="false" />.</returns>
		public bool ShouldSerializePriority() => Priority.HasValue;
	}
}