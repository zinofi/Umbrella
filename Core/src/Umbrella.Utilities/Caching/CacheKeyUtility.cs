using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Constants;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Utilities.Caching
{
	/// <summary>
	/// A utility used to create cache keys.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Caching.Abstractions.ICacheKeyUtility" />
	public class CacheKeyUtility : ICacheKeyUtility
	{
		private readonly ILogger _log;
		private readonly IDataLookupNormalizer _lookupNormalizer;

		/// <summary>
		/// Initializes a new instance of the <see cref="CacheKeyUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="lookupNormalizer">The lookup normalizer.</param>
		public CacheKeyUtility(
			ILogger<CacheKeyUtility> logger,
			IDataLookupNormalizer lookupNormalizer)
		{
			_log = logger;
			_lookupNormalizer = lookupNormalizer;
		}

		/// <inheritdoc />
		public string Create<T>(string key) => Create(typeof(T), key);

		/// <inheritdoc />
		public string Create(Type type, string key)
		{
			Guard.ArgumentNotNull(type, nameof(type));
			Guard.ArgumentNotNullOrWhiteSpace(key, nameof(key));

			char[] rentedArray = null;

			try
			{
				string typeName = type.FullName;
				int length = typeName.Length + key.Length + 1;

				bool isStack = length <= StackAllocConstants.MaxCharSize;

				Span<char> span = isStack ? stackalloc char[length] : rentedArray = ArrayPool<char>.Shared.Rent(length);
				span.Append(0, typeName);
				span.Append(typeName.Length, ":");
				span.Append(typeName.Length + 1, key);

				if (!isStack)
					span = span.Slice(0, length);

				return _lookupNormalizer.Normalize(span.ToString());
			}
			catch (Exception exc) when (_log.WriteError(exc, new { type, key }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem creating the cache key.", exc);
			}
			finally
			{
				if (rentedArray != null)
					ArrayPool<char>.Shared.Return(rentedArray);
			}
		}

		/// <inheritdoc />
		public string Create<T>(in ReadOnlySpan<string> keyParts, int? keyPartsLength = null) => Create(typeof(T), keyParts, keyPartsLength);

		/// <inheritdoc />
		public string Create(Type type, in ReadOnlySpan<string> keyParts, int? keyPartsLength = null)
		{
			Guard.ArgumentNotNull(type, nameof(type));
			Guard.ArgumentNotEmpty(keyParts, nameof(keyParts));
			Guard.ArgumentInRange(keyPartsLength, nameof(keyPartsLength), 1, keyParts.Length, allowNull: true);

			char[] rentedArray = null;

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

				bool isStack = length <= StackAllocConstants.MaxCharSize;

				Span<char> span = isStack ? stackalloc char[length] : rentedArray = ArrayPool<char>.Shared.Rent(length);

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

				if (!isStack)
					span = span.Slice(0, length);

				// This is the only part that allocates
				return _lookupNormalizer.Normalize(span.ToString());
			}
			catch (Exception exc) when (_log.WriteError(exc, new { type, keyParts = keyParts.ToArray(), keyPartsLength }, returnValue: true))
			{
				throw new UmbrellaException("There was a problem creating the cache key.", exc);
			}
			finally
			{
				if (rentedArray != null)
					ArrayPool<char>.Shared.Return(rentedArray);
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