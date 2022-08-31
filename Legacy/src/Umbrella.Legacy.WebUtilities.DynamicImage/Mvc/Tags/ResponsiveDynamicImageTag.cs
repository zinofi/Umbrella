// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Web;
using System.Web.Mvc;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.Legacy.WebUtilities.Mvc.Tags;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Mvc.Tags;

public class ResponsiveDynamicImageTag : ResponsiveImageTag
{
	#region Private Members
	private readonly HashSet<int> _sizeWidths = new();
	private string? _sizeAttributeValue;
	private readonly float _ratio;
	private readonly IDynamicImageUtility _dynamicImageUtility;
	private readonly DynamicResizeMode _resizeMode;
	private readonly DynamicImageFormat _format;
	private readonly string _dynamicImagePathPrefix;
	private readonly bool _toAbsolutePath;
	private readonly HttpRequestBase _httpRequest;
	private readonly string? _schemeOverride;
	private readonly string? _hostOverride;
	private readonly int _portOverride;
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="ResponsiveDynamicImageTag"/> class.
	/// </summary>
	/// <param name="dynamicImageUtility">The dynamic image utility.</param>
	/// <param name="dynamicImagePathPrefix">The dynamic image path prefix.</param>
	/// <param name="path">The path.</param>
	/// <param name="altText">The alt text.</param>
	/// <param name="width">The width.</param>
	/// <param name="height">The height.</param>
	/// <param name="resizeMode">The resize mode.</param>
	/// <param name="htmlAttributes">The HTML attributes.</param>
	/// <param name="format">The format.</param>
	/// <param name="mapVirtualPathFunc">The map virtual path function.</param>
	/// <param name="toAbsolutePath">if set to <c>true</c> [to absolute path].</param>
	/// <param name="request">The request.</param>
	/// <param name="schemeOverride">The scheme override.</param>
	/// <param name="hostOverride">The host override.</param>
	/// <param name="portOverride">The port override.</param>
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
		string? schemeOverride,
		string? hostOverride,
		int portOverride)
		: base(path, altText, htmlAttributes, mapVirtualPathFunc)
	{
		_dynamicImageUtility = dynamicImageUtility;
		_resizeMode = resizeMode;
		_format = format;
		_dynamicImagePathPrefix = dynamicImagePathPrefix;
		_toAbsolutePath = toAbsolutePath;
		_httpRequest = request;
		_schemeOverride = schemeOverride;
		_hostOverride = hostOverride;
		_portOverride = portOverride;

		_ratio = width / (float)height;

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
		if (_sizeWidths.Count == 0 && PixelDensities.Count > 1)
		{
			base.AddSrcsetAttribute(imgTag);
		}
		else if (_sizeWidths.Count > 0)
		{
			if (!string.IsNullOrWhiteSpace(_sizeAttributeValue))
				imgTag.Attributes["sizes"] = _sizeAttributeValue;

			imgTag.Attributes["srcset"] = string.Join(",", GetSizeStrings());
		}
	}
	#endregion

	#region Public Methods
	public ResponsiveDynamicImageTag WithSizes(params int[] x1Widths)
	{
		foreach (int size in x1Widths)
			_ = _sizeWidths.Add(size);

		return this;
	}

	public ResponsiveDynamicImageTag WithSizesAttributeValue(string value)
	{
		_sizeAttributeValue = value;

		return this;
	}
	#endregion

	#region Private Methods
	private IEnumerable<string> GetSizeStrings()
	{
		foreach (int sizeWidth in _sizeWidths)
		{
			foreach (int density in PixelDensities)
			{
				int imgWidth = sizeWidth * density;
				int imgHeight = (int)Math.Ceiling(imgWidth / _ratio);

				var options = new DynamicImageOptions(Path, imgWidth, imgHeight, _resizeMode, _format);

				string imgUrl = _dynamicImageUtility.GenerateVirtualPath(_dynamicImagePathPrefix, options);

				imgUrl = MapVirtualPathFunc(imgUrl);

				if (_toAbsolutePath)
					imgUrl = imgUrl.ToAbsoluteUrl(_httpRequest.Url, _schemeOverride, _hostOverride, _portOverride);

				yield return $"{imgUrl} {imgWidth}w";
			}
		}
	}
	#endregion
}