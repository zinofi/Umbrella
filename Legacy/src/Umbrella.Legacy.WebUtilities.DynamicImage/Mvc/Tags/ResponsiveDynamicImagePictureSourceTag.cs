using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Mvc.Tags;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Tags;

/// <summary>
/// A tag that can be used to render a <![CDATA[<source>]]> tag for use with a <![CDATA[<picture>]]> tag with a srcset attribute that has URLs using different pixel density values
/// using a well-known naming convention.
/// </summary>
/// <seealso cref="ResponsiveImageTag" />
public class ResponsiveDynamicImagePictureSourceTag : ResponsiveImageTag
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ResponsiveDynamicImagePictureSourceTag"/> class.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <param name="mediaAttributeValue">The media attribute value.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="mapVirtualPathFunc">The delegate used to map a virtual path to an application relative path.</param>
	public ResponsiveDynamicImagePictureSourceTag(
		string path,
		string mediaAttributeValue,
		IDictionary<string, object> htmlAttributes,
		Func<string, string> mapVirtualPathFunc)
		: base(path, string.Empty, htmlAttributes, mapVirtualPathFunc)
	{
		HtmlAttributes["media"] = mediaAttributeValue;
	}

	/// <inheritdoc />
	public override string ToHtmlString()
	{
		var tag = new TagBuilder("source");

		AddSrcsetAttribute(tag);

		//Remove obsolete values
		_ = HtmlAttributes.Remove("alt");
		_ = HtmlAttributes.Remove("src");

		tag.MergeAttributes(HtmlAttributes);

		return tag.ToString(TagRenderMode.SelfClosing);
	}
}