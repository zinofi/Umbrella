using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Umbrella.Utilities;
using Umbrella.Utilities.Imaging;

namespace Umbrella.AspNetCore.Blazor.Components.ResponsiveImage
{
	public abstract class UmbrellaResponsiveImageBase : ComponentBase
	{
		[Inject]
		protected IResponsiveImageHelper ResponsiveImageHelper { get; set; } = null!;

		[Parameter]
		public string? CssClass { get; set; }

		[Parameter]
		public virtual int MaxPixelDensity { get; set; } = 1;

		[Parameter]
		public virtual string Url { get; set; } = null!;

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = null!;

		public string? SrcSetValue { get; set; }

		protected override void OnParametersSet()
		{
			Guard.ArgumentNotNullOrWhiteSpace(Url, nameof(Url));
		}

		protected override void OnInitialized() => InitializeImage();

		protected virtual void InitializeImage()
		{
			SrcSetValue = ResponsiveImageHelper.GetPixelDensitySrcSetValue(Url, MaxPixelDensity);
		}
	}
}