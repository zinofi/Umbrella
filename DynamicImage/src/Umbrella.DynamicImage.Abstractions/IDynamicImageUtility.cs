using System.Collections.Generic;

namespace Umbrella.DynamicImage.Abstractions
{
	public interface IDynamicImageUtility
	{
		DynamicImageFormat ParseImageFormat(string format);
		(DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl, DynamicImageFormat? overrideFormat = null);
		bool ImageOptionsValid(DynamicImageOptions imageOptions, IEnumerable<DynamicImageMapping> validMappings);
		string GenerateVirtualPath(string dynamicImagePathPrefix, DynamicImageOptions options);
	}
}