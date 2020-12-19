using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Tags
{
	public class ResponsiveImageTag : IHtmlString
	{
		#region Protected Members		
		/// <summary>
		/// Gets the path.
		/// </summary>
		protected string Path { get; }

		/// <summary>
		/// Gets the map virtual path function.
		/// </summary>
		protected Func<string, string> MapVirtualPathFunc { get; }

		/// <summary>
		/// Gets the pixel densities.
		/// </summary>
		protected HashSet<int> PixelDensities { get; } = new HashSet<int> { 1 };

		/// <summary>
		/// Gets the HTML attributes.
		/// </summary>
		protected Dictionary<string, string> HtmlAttributes { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResponsiveImageTag"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="altText">The alt text.</param>
		/// <param name="htmlAttributes">The HTML attributes.</param>
		/// <param name="mapVirtualPath">The map virtual path.</param>
		public ResponsiveImageTag(string path, string altText, IDictionary<string, object>? htmlAttributes, Func<string, string> mapVirtualPath)
		{
			Path = path;
			MapVirtualPathFunc = mapVirtualPath;

			if (htmlAttributes == null)
				HtmlAttributes = new Dictionary<string, string>();
			else
				HtmlAttributes = htmlAttributes.ToDictionary(x => x.Key, x => x.Value.ToString());

			HtmlAttributes.Add("src", mapVirtualPath(path));
			HtmlAttributes.Add("alt", altText);
		}
		#endregion

		#region IHtmlString Members		
		/// <summary>
		/// Converts this tag to an HTML string.
		/// </summary>
		public virtual string ToHtmlString()
		{
			var imgTag = new TagBuilder("img");

			if (PixelDensities.Count > 1)
				AddSrcsetAttribute(imgTag);

			imgTag.MergeAttributes(HtmlAttributes);

			return imgTag.ToString(TagRenderMode.SelfClosing);
		}
		#endregion

		#region Public Methods
		public ResponsiveImageTag WithDensities(params int[] densities)
		{
			foreach (int density in densities)
				PixelDensities.Add(density);

			return this;
		}

		public ResponsiveImageTag WithFixedSize(int width)
		{
			string strWidth = width.ToString();

			HtmlAttributes["width"] = strWidth;
			HtmlAttributes["height"] = strWidth;

			return this;
		}

		public ResponsiveImageTag WithFixedSize(int width, int height)
		{
			HtmlAttributes["width"] = width.ToString();
			HtmlAttributes["height"] = height.ToString();

			return this;
		}
		#endregion

		#region Protected Methods
		protected virtual void AddSrcsetAttribute(TagBuilder imgTag)
		{
			string path = HtmlAttributes["src"];

			int densityIndex = path.LastIndexOf('.');

			IEnumerable<string> srcsetImagePaths =
				from density in PixelDensities
				orderby density
				let densityX = $"{density}x"
				let highResImagePath = density > 1 ? path.Insert(densityIndex, $"@{densityX}") : path
				select MapVirtualPathFunc(highResImagePath) + " " + densityX;

			imgTag.Attributes["srcset"] = string.Join(", ", srcsetImagePaths);
		}
		#endregion
	}
}
