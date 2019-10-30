using System.Web.Mvc;
using Microsoft.Extensions.Logging;

namespace Umbrella.Legacy.WebUtilities.Mvc
{
	public class UmbrellaController : Controller
	{
		#region Protected Properties
		protected ILogger Log { get; }
		#endregion

		#region Constructors
		public UmbrellaController(ILogger logger)
		{
			Log = logger;
		}
		#endregion
	}
}