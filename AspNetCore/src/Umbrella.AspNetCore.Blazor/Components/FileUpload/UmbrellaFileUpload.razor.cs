// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Tewr.Blazor.FileReader;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.Blazor.Components.FileUpload
{
	// TODO: Change the internal implementation to use the built-in InputFile component from .NET 5.0
	// Do this when migrating to .NET 6.0
	public partial class UmbrellaFileUpload : ComponentBase, IDisposable
	{
		private Stream? _fileStream;
		private CancellationTokenSource? _cancellationTokenSource;

		[Inject]
		private ILogger<UmbrellaFileUpload> Logger { get; set; } = null!;

		[Inject]
		private IUmbrellaDialogUtility DialogUtility { get; set; } = null!;

		[Inject]
		private IFileReaderService FileReaderService { get; set; } = null!;

		/// <summary>
		/// Gets or sets the maximum file size in bytes that can be uploaded.
		/// </summary>
		[Parameter]
		public int? MaxFileSizeBytes { get; set; }

		/// <summary>
		/// Gets or sets whether a warning message should be shown to the user when they clear the current file selection.
		/// </summary>
		[Parameter]
		public bool ShowClearWarning { get; set; } = true;

		/// <summary>
		/// Gets or sets whether a warning message should be shown to the user when they cancel the file upload.
		/// </summary>
		[Parameter]
		public bool ShowCancelWarning { get; set; } = true;

		/// <summary>
		/// Gets or sets a comma-delimited list of file extensions and/or MIME types that this component will accept.
		/// </summary>
		[Parameter]
		public string? Accept { get; set; }

		[Parameter]
		public EventCallback<UmbrellaFileUploadRequestEventArgs> OnRequestUpload { get; set; }

		private string Id { get; } = Guid.NewGuid().ToString();
		private ElementReference FileUploadReference { get; set; }
		private IFileReference? SelectedFile { get; set; }
		private IFileInfo? SelectedFileInfo { get; set; }
		private UmbrellaFileUploadStatus Status { get; set; }
		private string? AcceptTypesMessage { get; set; }
		private int? MaxFileSizeMegaBytes { get; set; }

		/// <inheritdoc />
		protected override async Task OnParametersSetAsync()
		{
			try
			{
				if (MaxFileSizeBytes.HasValue)
					MaxFileSizeMegaBytes = MaxFileSizeBytes / 1024 / 1024;

				if (!string.IsNullOrWhiteSpace(Accept))
				{
					string[] types = Accept.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimToLowerInvariant()).Distinct().ToArray();

					if (types.Length is 0)
					{
						Accept = null;
						return;
					}

					// NB: The types[..^1] was originally types.SkipLast(1)
					// The types[^1] was originally types[types.Length - 1]. The syntax is using the new C#8 Index stuff. ^1 effectively means get the item at the end.
					AcceptTypesMessage = types.Length is 1
						? $"Please select a {types[0]} file to upload."
						: $"Please select a {string.Join(", ", types[..^1])} or a {types[^1]} file to upload.";
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		private async Task OnFileSelected()
		{
			try
			{
				IEnumerable<IFileReference> lstFile = await FileReaderService.CreateReference(FileUploadReference).EnumerateFilesAsync();

				var file = lstFile.FirstOrDefault();

				if (file != null)
				{
					IFileInfo fileInfo = await file.ReadFileInfoAsync();

					if (fileInfo.Size > MaxFileSizeBytes)
					{
						await DialogUtility.ShowDangerMessageAsync($"Please select a file with a maximum size of {MaxFileSizeMegaBytes} MB.", "Maximum File Size Exceeded");
						return;
					}

					SelectedFile = file;
					SelectedFileInfo = fileInfo;
					_fileStream = await file.OpenReadAsync();
					Status = UmbrellaFileUploadStatus.Selected;
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		private async Task UploadClick()
		{
			void PositionChanged(object? sender, IFilePositionInfo args) => InvokeAsync(StateHasChanged);

			try
			{
				if (SelectedFileInfo is null || SelectedFile is null || _fileStream is null)
					throw new Exception("File Info should not be null here.");

				_cancellationTokenSource = new CancellationTokenSource();
				Status = UmbrellaFileUploadStatus.Uploading;

				SelectedFileInfo.PositionInfo.PositionChanged += PositionChanged;

				if (OnRequestUpload.HasDelegate)
					await OnRequestUpload.InvokeAsync(new UmbrellaFileUploadRequestEventArgs(_fileStream, SelectedFileInfo.Name, SelectedFileInfo.Type, _cancellationTokenSource.Token));

				ClearSelection();
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				Cleanup();
				await DialogUtility.ShowDangerMessageAsync("There has been a problem uploading the selected file. Please try again.");
			}
			finally
			{
				if (SelectedFileInfo != null)
					SelectedFileInfo.PositionInfo.PositionChanged -= PositionChanged;
			}
		}

		private async Task ClearClick()
		{
			try
			{
				if (ShowClearWarning)
				{
					bool clear = await DialogUtility.ShowConfirmWarningMessageAsync("Are you sure you want to clear the current file and choose a new one?", "Clear Selection");

					if (!clear)
						return;
				}

				ClearSelection();
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		private async Task CancelClick()
		{
			try
			{
				if (ShowCancelWarning)
				{
					bool cancel = await DialogUtility.ShowConfirmWarningMessageAsync("Are you sure you want to cancel the file upload?", "Cancel Upload");

					if (!cancel)
						return;
				}

				Cleanup();
			}
			catch (Exception exc) when (Logger.WriteError(exc, returnValue: true))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		/// <inheritdoc />
		public void Dispose() => Cleanup();

		private void Cleanup()
		{
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}

			Status = UmbrellaFileUploadStatus.Selected;
		}

		private void ClearSelection()
		{
			Cleanup();

			SelectedFile = null;
			SelectedFileInfo = null;
			Status = UmbrellaFileUploadStatus.None;

			if (_fileStream != null)
			{
				_fileStream.Dispose();
				_fileStream = null;
			}
		}
	}
}