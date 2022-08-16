// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Utilities.Options.Abstractions;

/// <summary>
/// An interface used to allow Umbrella Options types to be validated.
/// </summary>
public interface IValidatableUmbrellaOptions
{
	/// <summary>
	/// Validates this instance.
	/// </summary>
	void Validate();
}