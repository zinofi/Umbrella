using Moq;
using Umbrella.Utilities.Data.Abstractions;

namespace Umbrella.Internal.Mocks
{
	public static class CoreUtilitiesMocks
	{
		public static ILookupNormalizer CreateILookupNormalizer()
		{
			var lookupNormalizer = new Mock<ILookupNormalizer>();
			lookupNormalizer.Setup(x => x.Normalize(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((value, trim) =>
			{
				if (value is null)
					return null;

				return trim ? value.Trim().ToUpperInvariant() : value.ToUpperInvariant();
			});

			return lookupNormalizer.Object;
		}
	}
}