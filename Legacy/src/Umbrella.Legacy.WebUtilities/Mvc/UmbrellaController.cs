using System.Web.Mvc;
using Microsoft.Extensions.Logging;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
	/// <summary>
	/// Serves as the base class for MVC controllers and encapsulates MVC specific functionality.
	/// </summary>
	public abstract class UmbrellaController : Controller
	{
		#region Protected Properties		
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaController"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public UmbrellaController(ILogger logger)
		{
			Log = logger;
		}
		#endregion
	}
}