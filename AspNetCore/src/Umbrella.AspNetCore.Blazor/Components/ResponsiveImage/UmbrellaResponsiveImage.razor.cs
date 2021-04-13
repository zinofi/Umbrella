using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Umbrella.Utilities;
using Umbrella.Utilities.Imaging;

namespace Umbrella.AspNetCore.Blazor.Components.ResponsiveImage
{
	public abstract class UmbrellaResponsiveImageBase : ComponentBase
	{
		/// <summary>
		/// Gets or sets the responsive image helper.
		/// </summary>
		[Inject]
		protected IResponsiveImageHelper ResponsiveImageHelper { get; set; } = null!;

		/// <summary>
		/// Gets or sets the CSS class that is applied to the div that wraps the generated img tag.
		/// To apply a CSS class directly to the img tag, use the "class" attribute.
		/// </summary>
		[Parameter]
		public string? CssClass { get; set; }

		/// <summary>
		/// Gets or sets the maximum pixel density. Defaults to 1.
		/// </summary>
		[Parameter]
		public virtual int MaxPixelDensity { get; set; } = 1;

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		[Parameter]
		public virtual string Url { get; set; } = null!;

		/// <summary>
		/// Gets or sets the additional attributes.
		/// </summary>
		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = null!;

		/// <summary>
		/// Gets or sets the source set value used to populate the srcset attribute.
		/// </summary>
		protected string? SrcSetValue { get; set; }

		/// <inheritdoc />
		protected override void OnParametersSet() => Guard.ArgumentNotNullOrWhiteSpace(Url, nameof(Url));

		/// <inheritdoc />
		protected override void OnInitialized() => InitializeImage();

		/// <summary>
		/// Initializes the image.
		/// </summary>
		protected virtual void InitializeImage() => SrcSetValue = ResponsiveImageHelper.GetPixelDensitySrcSetValue(Url, MaxPixelDensity);

		/// <inheritdoc />
		protected override void OnAfterRender(bool firstRender)
		{
			if (!firstRender)
				InitializeImage();
		}
	}
}