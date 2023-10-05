namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// A resolver utility used to resolve instances of types used by the Dynamic Image infrastructure.
/// </summary>
/// <remarks>This needs to be kept as it's used by the N2 packages.</remarks>
public interface IDynamicImageUtilityResolver
{
	/// <summary>
	/// Gets the instance.
	/// </summary>
	IDynamicImageUtility Instance { get; }
}