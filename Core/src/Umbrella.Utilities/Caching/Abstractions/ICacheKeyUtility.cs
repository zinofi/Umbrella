using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities.Caching.Abstractions
{
    public interface ICacheKeyUtility
    {
        string Create<T>(IList<string> keyParts, [CallerMemberName] string callerMemberName = "");
    }
}