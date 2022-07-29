using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
	/// <summary>
	/// Serves as the base class for all MVC View Components.
	/// </summary>
	public abstract class UmbrellaViewComponent : ViewComponent
	{
		#region Protected Members		
		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Logger { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaViewComponent"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public UmbrellaViewComponent(ILogger logger)
		{
			Logger = logger;
		}
		#endregion
	}
}