using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Utilities.Threading.Redis.Options;

/// <summary>
/// Options for use with the <see cref="DistributedRedisSynchronizationManager"/>.
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public sealed class DistributedRedisSynchronizationManagerOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the Redis connection string.
	/// </summary>
	public string ConnectionString { get; set; } = null!;

	/// <inheritdoc />
	public void Sanitize() => ConnectionString = ConnectionString?.Trim()!;

	/// <inheritdoc />
	public void Validate() => Guard.IsNotNullOrEmpty(ConnectionString);
}