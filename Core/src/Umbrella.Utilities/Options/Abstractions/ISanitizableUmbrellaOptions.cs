// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Utilities.Options.Abstractions;

/// <summary>
/// An interface used to allow Umbrella Options types to be sanitized.
/// </summary>
public interface ISanitizableUmbrellaOptions
{
	/// <summary>
	/// Sanitizes this instance.
	/// </summary>
	void Sanitize();
}