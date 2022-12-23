// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Reflection;
using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Mapping.Mapperly.Abstractions;
using Umbrella.Utilities.Mapping.Mapperly.Enumerations;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Utilities.Mapping.Mapperly.Options;

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

	/// <summary>
	/// Gets or sets the environment.
	/// </summary>
	/// <remarks>Defaults to <see cref="MapperlyEnvironmentType.Server"/>.</remarks>
	public MapperlyEnvironmentType Environment { get; set; } = MapperlyEnvironmentType.Server;

	/// <inheritdoc/>
	void ISanitizableUmbrellaOptions.Sanitize() => TargetAssemblies = TargetAssemblies?.Distinct().ToArray()!;

	/// <inheritdoc/>
	void IValidatableUmbrellaOptions.Validate()
	{
		Guard.IsNotNull(TargetAssemblies);
		Guard.HasSizeGreaterThan(TargetAssemblies, 0);
	}
}