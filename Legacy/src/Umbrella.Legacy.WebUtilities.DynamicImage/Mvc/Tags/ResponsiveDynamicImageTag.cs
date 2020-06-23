using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.Legacy.WebUtilities.Mvc.Tags;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Tags
{
	public class ResponsiveDynamicImageTag : ResponsiveImageTag
	{
		#region Private Members
		private readonly HashSet<int> m_SizeWidths = new HashSet<int>();
		private string m_SizeAttributeValue;
		private readonly float m_Ratio;
		private readonly IDynamicImageUtility m_DynamicImageUtility;
		private readonly DynamicResizeMode m_ResizeMode;
		private readonly DynamicImageFormat m_Format;
		private readonly string m_DynamicImagePathPrefix;
		private readonly bool m_ToAbsolutePath;
		private readonly HttpRequestBase m_HttpRequest;
		private readonly string m_SchemeOverride;
		private readonly string m_HostOverride;
		private readonly int m_PortOverride;
		#endregion

		#region Constructors
		public ResponsiveDynamicImageTag(
			IDynamicImageUtility dynamicImageUtility,
			string dynamicImagePathPrefix,
			string path,
			string altText,
			int width,
			int height,
			DynamicResizeMode resizeMode,
			IDictionary<string, object> htmlAttributes,
			DynamicImageFormat format,
			Func<string, string> mapVirtualPathFunc,
			bool toAbsolutePath,
			HttpRequestBase request,
			string schemeOverride,
			string hostOverride,
			int portOverride)
			: base(path, altText, htmlAttributes, mapVirtualPathFunc)
		{
			m_DynamicImageUtility = dynamicImageUtility;
			m_ResizeMode = resizeMode;
			m_Format = format;
			m_DynamicImagePathPrefix = dynamicImagePathPrefix;
			m_ToAbsolutePath = toAbsolutePath;
			m_HttpRequest = request;
			m_SchemeOverride = schemeOverride;
			m_HostOverride = hostOverride;
			m_PortOverride = portOverride;

			m_Ratio = width / (float)height;

			var options = new DynamicImageOptions(path, width, height, resizeMode, format);

			string x1Url = dynamicImageUtility.GenerateVirtualPath(dynamicImagePathPrefix, options);

			string relativePath = mapVirtualPathFunc(x1Url);

			HtmlAttributes["src"] = toAbsolutePath ? relativePath.ToAbsoluteUrl(request.Url, schemeOverride, hostOverride, portOverride) : relativePath;
		}
		#endregion

		#region Overridden Methods
		public override string ToHtmlString()
		{
			var imgTag = new TagBuilder("img");

			AddSrcsetAttribute(imgTag);

			imgTag.MergeAttributes(HtmlAttributes);

			return imgTag.ToString(TagRenderMode.SelfClosing);
		}

		protected override void AddSrcsetAttribute(TagBuilder imgTag)
		{
			//If we don't have any size information just call into the base method
			//but only if we also have some density information
			if (m_SizeWidths.Count == 0 && PixelDensities.Count > 1)
			{
				base.AddSrcsetAttribute(imgTag);
			}
			else if (m_SizeWidths.Count > 0)
			{
				if (!string.IsNullOrWhiteSpace(m_SizeAttributeValue))
					imgTag.Attributes["sizes"] = m_SizeAttributeValue;

				imgTag.Attributes["srcset"] = string.Join(",", GetSizeStrings());
			}
		}
		#endregion

		#region Public Methods
		public ResponsiveDynamicImageTag WithSizes(params int[] x1Widths)
		{
			foreach (int size in x1Widths)
				m_SizeWidths.Add(size);

			return this;
		}

		public ResponsiveDynamicImageTag WithSizesAttributeValue(string value)
		{
			m_SizeAttributeValue = value;

			return this;
		}
		#endregion

		#region Private Methods
		private IEnumerable<string> GetSizeStrings()
		{
			foreach (int sizeWidth in m_SizeWidths)
			{
				foreach (int density in PixelDensities)
				{
					int imgWidth = sizeWidth * density;
					int imgHeight = (int)Math.Ceiling(imgWidth / m_Ratio);

					var options = new DynamicImageOptions(Path, imgWidth, imgHeight, m_ResizeMode, m_Format);

					string imgUrl = m_DynamicImageUtility.GenerateVirtualPath(m_DynamicImagePathPrefix, options);

					imgUrl = MapVirtualPathFunc(imgUrl);

					if (m_ToAbsolutePath)
						imgUrl = imgUrl.ToAbsoluteUrl(m_HttpRequest.Url, m_SchemeOverride, m_HostOverride, m_PortOverride);

					yield return $"{imgUrl} {imgWidth}w";
				}
			}
		}
		#endregion
	}
}