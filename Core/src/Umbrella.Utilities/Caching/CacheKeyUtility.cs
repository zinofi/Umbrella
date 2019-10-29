using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Caching
{
	public class CacheKeyUtility : ICacheKeyUtility
	{
		private readonly ILogger _log;

		public CacheKeyUtility(ILogger<CacheKeyUtility> logger)
		{
			_log = logger;
		}

		public string Create<T>(string key) => Create(typeof(T), key);

		public string Create(Type type, string key)
		{
			Guard.ArgumentNotNull(type, nameof(type));
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			try
			{
				string typeName = type.FullName;

				Span<char> span = stackalloc char[typeName.Length + key.Length + 1];
				span.Append(0, typeName);
				span.Append(typeName.Length, ":");
				span.Append(typeName.Length + 1, key);
				span.ToUpperInvariant();

				return span.ToString();
			}
			catch (Exception exc) when (_log.WriteError(exc, new { type, key }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem creating the cache key.", exc);
			}
		}

		public string Create<T>(in ReadOnlySpan<string> keyParts, int? keyPartsLength = null) => Create(typeof(T), keyParts, keyPartsLength);

		public string Create(Type type, in ReadOnlySpan<string> keyParts, int? keyPartsLength = null)
		{
			Guard.ArgumentNotNull(type, nameof(type));
			Guard.ArgumentNotEmpty(keyParts, nameof(keyParts));
			Guard.ArgumentInRange(keyPartsLength, nameof(keyPartsLength), 1, keyParts.Length, allowNull: true);

			try
			{
				int partsCount = keyPartsLength ?? keyParts.Length;

				// It seems the typeof call is expensive on CLR vs .NET Core
				string typeName = type.FullName;
				int partsLengthTotal = -1;

				for (int i = 0; i < partsCount; i++)
				{
					string part = keyParts[i];

					if (part != null)
						partsLengthTotal += part.Length + 1;
				}

				int length = typeName.Length + partsLengthTotal + 1;

				Span<char> span = stackalloc char[length];

				int currentIndex = span.Append(0, typeName);
				span[currentIndex++] = ':';

				for (int i = 0; i < partsCount; i++)
				{
					string part = keyParts[i];

					if (part != null)
					{
						currentIndex = span.Append(currentIndex, part);

						if (i < partsCount - 1)
							span[currentIndex++] = ':';
					}
				}

				span.ToUpperInvariant();

				// This is the only part that allocates
				return span.ToString();
			}
			catch (Exception exc) when (_log.WriteError(exc, new { type, keyParts = keyParts.ToArray(), keyPartsLength }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem creating the cache key.", exc);
			}
		}

#if !AzureDevOps
		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal string CreateOld<T>(IEnumerable<string> keyParts)
		{
			Guard.ArgumentNotNullOrEmpty(keyParts, nameof(keyParts));

			return $"{typeof(T).FullName}:{string.Join(":", keyParts)}".ToUpperInvariant();
		}
#endif
	}
}