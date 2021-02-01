using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	/// <summary>
	/// A utility used to perform navigation to a specific URI.
	/// </summary>
    public interface IUriNavigator
    {
		/// <summary>
		/// Opens the specified URI on the current device.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="openInNewWindow">Specifies whether or not to open the <paramref name="uri"/> in the current or a new window.</param>
		/// <returns>An awaitable task that completes when the URI has been navigated to.</returns>
		ValueTask OpenAsync(string uri, bool openInNewWindow);
    }
}