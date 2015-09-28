using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbrella.WebUtilities.DynamicImage;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
	public static class ImageHelpers
	{
		public static ResponsiveImageTag Image(this HtmlHelper helper, string path, string altText, string cssClass = "")
		{
			UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

			return new ResponsiveImageTag(path, altText, cssClass, urlHelper.Content);
		}

		public static ResponsiveImageTag DynamicImage(this HtmlHelper helper, string path, string altText, int width, int height, DynamicResizeMode resizeMode, string cssClass = "", DynamicImageFormat format = DynamicImageFormat.Jpeg, bool toAbsolutePath = false)
		{
			string imageUrl = DynamicImageUtility.GetResizedUrl(path, width, height, resizeMode, format, toAbsolutePath);

			return helper.Image(imageUrl, altText, cssClass);
		}
	}

	public class ResponsiveImageTag : IHtmlString
	{
		private readonly string m_Path;
		private readonly Func<string, string> m_MapVirtualPathFunc;
		private readonly HashSet<int> m_PixelDensities;
		private readonly Dictionary<string, string> m_HtmlAttributes;

		public ResponsiveImageTag(string path, string altText, string cssClass, Func<string, string> mapVirtualPath)
		{
			m_Path = path;
			m_MapVirtualPathFunc = mapVirtualPath;

			m_PixelDensities = new HashSet<int>();
			m_HtmlAttributes = new Dictionary<string, string>
			{
				{ "src", mapVirtualPath(path) },
				{ "alt", altText },
				{ "class", cssClass }
			};
		}

		public string ToHtmlString()
		{
			var imgTag = new TagBuilder("img");

			if (m_PixelDensities.Count > 0)
				AddSrcsetAttribute(imgTag);

			foreach (KeyValuePair<string, string> attribute in m_HtmlAttributes)
			{
				if (!string.IsNullOrWhiteSpace(attribute.Value))
					imgTag.Attributes[attribute.Key] = attribute.Value;
			}

			return imgTag.ToString(TagRenderMode.SelfClosing);
		}

		private void AddSrcsetAttribute(TagBuilder imgTag)
		{
			int densityIndex = m_Path.LastIndexOf('.');

			IEnumerable<string> srcsetImagePaths =
				from density in m_PixelDensities
				let densityX = density + "x"
				let highResImagePath = m_Path.Insert(densityIndex, "@" + densityX) + " " + densityX
				select m_MapVirtualPathFunc(highResImagePath);

			imgTag.Attributes["srcset"] = string.Join(", ", srcsetImagePaths);
		}

		public ResponsiveImageTag WithDensities(params int[] densities)
		{
			foreach (int density in densities)
				m_PixelDensities.Add(density);

			return this;
		}

		public ResponsiveImageTag WithSize(int width, int? height = null)
		{
			m_HtmlAttributes["width"] = width.ToString();
			m_HtmlAttributes["height"] = (height ?? width).ToString();

			return this;
		}
	}
}