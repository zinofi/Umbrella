using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities.Caching.Abstractions
{
    public interface ICacheKeyUtility
    {
        string Create<T>(string key);
        string Create(Type type, string key);
        string Create<T>(in ReadOnlySpan<string> keyParts, int? keyPartsLength = null);
        string Create(Type type, in ReadOnlySpan<string> keyParts, int? keyPartsLength = null);
    }
}