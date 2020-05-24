namespace Umbrella.DynamicImage.Abstractions
{
	// NB: Keep this as it's used by the N2 CMS packages.
	public interface IDynamicImageUtilityResolver
	{
		IDynamicImageUtility Instance { get; }
	}
}