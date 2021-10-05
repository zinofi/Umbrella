using Microsoft.Extensions.Logging;
using Umbrella.Utilities.WeakEventManager.Abstractions;

namespace Umbrella.Utilities.WeakEventManager
{
	/// <summary>
	/// An event manager that allows subscribers to be subject to GC when still subscribed.
	/// </summary>
	/// <seealso cref="IGlobalWeakEventManager" />
	public class GlobalWeakEventManager : WeakEventManager, IGlobalWeakEventManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GlobalWeakEventManager"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public GlobalWeakEventManager(ILogger<WeakEventManager> logger)
			: base(logger)
		{
		}
	}
}