// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.AspNetCore.Blazor.Components.FileUpload;

/// <summary>
/// Represents the current status of the <see cref="UmbrellaFileUpload"/> component.
/// </summary>
public enum UmbrellaFileUploadStatus
{
	/// <summary>
	/// The initial status when no file has been uploaded or selected.
	/// </summary>
	None,

	Selected,
	Uploading,
	Uploaded
}