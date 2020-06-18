using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

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

		public static ICacheKeyUtility CreateCacheKeyUtility() => new CacheKeyUtility(new Mock<ILogger<CacheKeyUtility>>().Object, CreateILookupNormalizer());

		public static IHybridCache CreateHybridCache(bool enableCaching = true)
		{
			return new HybridCache(
				new Mock<ILogger<HybridCache>>().Object,
				new HybridCacheOptions { CacheEnabled = enableCaching },
				CreateILookupNormalizer(),
				new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
				new MemoryCache(Options.Create(new MemoryCacheOptions())));
		}

		public static IGenericTypeConverter CreateGenericTypeConverter()
		{
			var genericTypeConverter = new Mock<IGenericTypeConverter>();
			genericTypeConverter.Setup(x => x.Convert(It.IsAny<string>(), (string)null, null)).Returns<string, string, Func<string, string>>((x, y, z) => x);

			return genericTypeConverter.Object;
		}

		public static ILogger<T> CreateLogger<T>() => new Mock<ILogger<T>>().Object;

		public static ILoggerFactory CreateLoggerFactory<T>()
		{
			var loggerFactory = new Mock<ILoggerFactory>();
			loggerFactory.Setup(x => x.CreateLogger(typeof(T).FullName)).Returns(CreateLogger<T>());

			return loggerFactory.Object;
		}

		public static IMimeTypeUtility CreateMimeTypeUtility(params (string extension, string mimeType)[] mappings)
		{
			var mimeTypeUtility = new Mock<IMimeTypeUtility>();
			mappings.ForEach(mapping => mimeTypeUtility.Setup(x => x.GetMimeType(It.Is<string>(y => !string.IsNullOrEmpty(y) && y.Trim().ToLowerInvariant().EndsWith(mapping.extension)))).Returns(mapping.mimeType));

			return mimeTypeUtility.Object;
		}
	}
}