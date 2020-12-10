using System;
using System.Threading.Tasks;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	/// <summary>
	/// A utility used to show a message to the user when an application update is available and optionally force
	/// the user to upgrade to the new version.
	/// </summary>
	public interface IAppUpdateMessageUtility
	{
		/// <summary>
		/// Occurs when the message is shown.
		/// </summary>
		event Func<bool, string, Task> OnShow;

		/// <summary>
		/// Shows the update message.
		/// </summary>
		/// <param name="updateRequired">if set to <c>true</c> the update is required.</param>
		/// <param name="message">The message.</param>
		/// <returns>A task that completes when the message has been actioned by the user.</returns>
		ValueTask ShowAsync(bool updateRequired, string message);
	}
}