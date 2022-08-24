// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// Represents a DynamicImage.
/// </summary>
public class DynamicImageItem
{
	private DateTimeOffset? _lastModified;

	/// <summary>
	/// Gets or sets the last modified date
	/// </summary>
	public DateTimeOffset? LastModified
	{
		get => UmbrellaFileInfo != null ? UmbrellaFileInfo.LastModified : _lastModified;
		set => _lastModified = value;
	}

	/// <summary>
	/// Gets the length of the image.
	/// </summary>
	public long Length => UmbrellaFileInfo?.Length ?? Content?.Length ?? -1;

	/// <summary>
	/// Gets or sets the image options.
	/// </summary>
	public DynamicImageOptions ImageOptions { get; set; }

	/// <summary>
	/// Gets or sets the content.
	/// </summary>
	public byte[]? Content { private get; set; }

	/// <summary>
	/// Gets or sets the <see cref="IUmbrellaFileInfo"/> instance.
	/// </summary>
	public IUmbrellaFileInfo? UmbrellaFileInfo { get; set; }

	/// <summary>
	/// Gets the content of the image file.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The image file content.</returns>
	public async Task<byte[]?> GetContentAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (Content is null && UmbrellaFileInfo != null)
			Content = await UmbrellaFileInfo.ReadAsByteArrayAsync(cancellationToken);

		return Content;
	}

	/// <summary>
	/// Writes the content of the image file to the target stream.
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/>.</returns>
	/// <remarks>This is just a convenience wrapper around the <see cref="IUmbrellaFileInfo.WriteToStreamAsync(Stream, CancellationToken, int?)"/> method.</remarks>
	public Task WriteContentToStreamAsync(Stream target, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNull(target);

		if (Content is null && UmbrellaFileInfo != null)
			return UmbrellaFileInfo.WriteToStreamAsync(target, cancellationToken);

		return Content != null ? target.WriteAsync(Content, 0, Content.Length) : Task.CompletedTask;
	}
}