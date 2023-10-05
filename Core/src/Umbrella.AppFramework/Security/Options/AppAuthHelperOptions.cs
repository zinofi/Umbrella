// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AppFramework.Security.Options;

/// <summary>
/// Options for use with the <see cref="AppAuthHelper"/> class.
/// </summary>
/// <seealso cref="IValidatableUmbrellaOptions" />
public class AppAuthHelperOptions : IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets the post logout action.
	/// </summary>
	public Func<ValueTask>? PostLogoutAction { get; set; }

	/// <inheritdoc />
	public void Validate() => Guard.IsNotNull(PostLogoutAction);
}