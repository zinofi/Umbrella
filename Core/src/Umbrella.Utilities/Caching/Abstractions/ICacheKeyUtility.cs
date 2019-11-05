using System;

namespace Umbrella.Utilities.Caching.Abstractions
{
	/// <summary>
	/// A utility used to create cache keys.
	/// </summary>
	public interface ICacheKeyUtility
	{
		string Create<T>(string key);
		string Create(Type type, string key);
		string Create<T>(in ReadOnlySpan<string> keyParts, int? keyPartsLength = null);
		string Create(Type type, in ReadOnlySpan<string> keyParts, int? keyPartsLength = null);
	}
}