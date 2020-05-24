using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
	/// <summary>
	/// Serves as the base class for API controllers and encapsulates API specific functionality.
	/// </summary>
	[ApiController]
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
		protected IWebHostEnvironment HostingEnvironment { get; }

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
			IWebHostEnvironment hostingEnvironment)
		{
			Log = logger;
			HostingEnvironment = hostingEnvironment;

		}
		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a 201 Created <see cref="StatusCodeResult"/>.
		/// </summary>
		/// <returns>A <see cref="StatusCodeResult"/> of 201.</returns>
		protected virtual StatusCodeResult Created() => StatusCode(201);

		/// <summary>
		/// Creates a 201 Created <see cref="CreatedResult"/> with the specified content.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns>A <see cref="IActionResult"/> of 201.</returns>
		protected virtual IActionResult Created(object content) => Created("", content);

		/// <summary>
		/// Creates a 500 InternalServerError <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="IActionResult"/> of 500.</returns>
		protected virtual IActionResult InternalServerError(string reason) => StatusCode(500, reason);

		/// <summary>
		/// Creates a 403 Forbidden <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="IActionResult"/> of 403.</returns>
		protected virtual IActionResult Forbidden(string reason) => StatusCode(403, reason);

		/// <summary>
		/// Creates a 405 MethodNotAllowed <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="IActionResult"/> of 405.</returns>
		protected virtual IActionResult MethodNotAllowed(string reason) => StatusCode(405, reason);

		/// <summary>
		/// Creates a 429 TooManyRequests <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="IActionResult"/> of 429.</returns>
		protected virtual IActionResult TooManyRequests(string reason) => StatusCode(429, reason);
		#endregion
	}
}