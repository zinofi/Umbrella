using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.FileUpload;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Components.FileImagePreviewUpload
{
	public partial class UmbrellaFileImagePreviewUpload : ComponentBase
	{
		[Inject]
		private ILogger<UmbrellaFileImagePreviewUpload> Logger { get; set; } = null!;

		[Inject]
		private IUmbrellaDialogUtility DialogUtility { get; set; } = null!;

		[Parameter]
		public string ChangesMadeMessage { get; set; } = "You have made changes to this image. These changes will be saved when this page is saved.";

		[Parameter]
		public int? MaxFileSizeBytes { get; set; }

		[Parameter]
		public bool ShowClearWarning { get; set; } = true;

		[Parameter]
		public bool ShowCancelWarning { get; set; } = true;

		[Parameter]
		public string? Accept { get; set; }

		[Parameter]
		public EventCallback<UmbrellaFileUploadRequestEventArgs> OnRequestUpload { get; set; }

		[Parameter]
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

		[Parameter]
		public int WidthRequest { get; set; } = 1;

		[Parameter]
		public int HeightRequest { get; set; } = 1;

		[Parameter]
		public DynamicResizeMode ResizeMode { get; set; } = DynamicResizeMode.UniformFill;

		[Parameter]
		public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;

		[Parameter]
		public string? SizeWidths { get; set; }

		[Parameter]
		public int MaxPixelDensity { get; set; } = 1;

		[Parameter]
		public string? StripPrefix { get; set; }

		[Parameter]
		public string? Url { get; set; }

		[Parameter]
		public EventCallback OnDeleteImage { get; set; }

		private string? UpdatedImageUrl { get; set; }
		private UmbrellaFileImagePreviewUploadMode FileUploadMode { get; set; }

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			if (!string.IsNullOrWhiteSpace(Url))
			{
				UpdatedImageUrl = Url;
				FileUploadMode = UmbrellaFileImagePreviewUploadMode.Current;
			}
		}

		private async Task OnRequestUploadInner(UmbrellaFileUploadRequestEventArgs args)
		{
			try
			{
				if (OnRequestUpload.HasDelegate)
					await OnRequestUpload.InvokeAsync(args);

				StateHasChanged();
			}
			catch (Exception exc) when (Logger.WriteError(exc))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		private async Task DeleteImageClick()
		{
			try
			{
				bool delete = await DialogUtility.ShowConfirmDangerMessageAsync("Are you sure you want to delete this image? This change will not take effect until this page is saved.", "Delete Image");

				if (!delete)
					return;

				UpdatedImageUrl = null;
				FileUploadMode = UmbrellaFileImagePreviewUploadMode.Upload;

				if (OnDeleteImage.HasDelegate)
					await OnDeleteImage.InvokeAsync(EventArgs.Empty);
			}
			catch (Exception exc) when (Logger.WriteError(exc))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		public void Update(string? url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				UpdatedImageUrl = null;
				FileUploadMode = UmbrellaFileImagePreviewUploadMode.Upload;
			}
			else
			{
				string? currentImageUrl = UpdatedImageUrl;
				UpdatedImageUrl = url;
				FileUploadMode = currentImageUrl?.Equals(UpdatedImageUrl, StringComparison.OrdinalIgnoreCase) == true ? UmbrellaFileImagePreviewUploadMode.Current : UmbrellaFileImagePreviewUploadMode.New;
			}
		}
	}
}