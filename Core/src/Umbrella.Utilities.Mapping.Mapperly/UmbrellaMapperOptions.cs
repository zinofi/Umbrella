// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using System.Reflection;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Utilities.Mapping.Mapperly;

/// <summary>
/// Options for use with the <see cref="UmbrellaMapper"/>.
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class UmbrellaMapperOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the target assemblies to scan for types that implement <see cref="IUmbrellaMapperlyMapper{TSource, TDestination}"/>.
	/// </summary>
	public IReadOnlyCollection<Assembly> TargetAssemblies { get; set; } = Array.Empty<Assembly>();

	/// <inheritdoc/>
	void ISanitizableUmbrellaOptions.Sanitize() => TargetAssemblies = TargetAssemblies?.Distinct().ToArray()!;

	/// <inheritdoc/>
	void IValidatableUmbrellaOptions.Validate()
	{
		Guard.IsNotNull(TargetAssemblies);
		Guard.HasSizeGreaterThan(TargetAssemblies, 0);
	}
}