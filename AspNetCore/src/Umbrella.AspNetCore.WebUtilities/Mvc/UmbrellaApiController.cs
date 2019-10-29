using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.DataAccess.Abstractions.Exceptions;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
	/// <summary>
	/// Serves as the base class for API controllers and encapsulates API specific functionality.
	/// </summary>
	public abstract class UmbrellaApiController : ControllerBase
	{
		#region Protected Properties		
		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Log { get; }

		/// <summary>
		/// Gets the hosting environment.
		/// </summary>
		protected IHostingEnvironment HostingEnvironment { get; }

		/// <summary>
		/// Gets a value indicating whether the application is running in development mode.
		/// </summary>
		protected bool IsDevelopment => HostingEnvironment.IsDevelopment();
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaApiController"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="hostingEnvironment">The hosting environment.</param>
		public UmbrellaApiController(
			ILogger logger,
			IHostingEnvironment hostingEnvironment)
		{
			Log = logger;
			HostingEnvironment = hostingEnvironment;
		}
		#endregion

		// TODO: Review the legacy controller
		#region Public Methods
		[NonAction]
		public virtual IActionResult Created() => StatusCode(201);

		[NonAction]
		public virtual IActionResult Forbidden(string message = null) => HttpObjectOrStatusResult(message, 403);

		[NonAction]
		public virtual IActionResult Conflict(string message = null) => HttpObjectOrStatusResult(message, 409);

		[NonAction]
		public virtual IActionResult InternalServerError(string message = null) => HttpObjectOrStatusResult(message, 500, true);

		[NonAction]
		public virtual IActionResult HttpObjectOrStatusResult(string message, int statusCode, bool wrapMessage = false)
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				object value = message;

				if (wrapMessage)
					value = new { message };

				return StatusCode(statusCode, value);
			}

			return StatusCode(statusCode);
		}
		#endregion
	}
}