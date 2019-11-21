using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

		// TODO: Review these - they are likely redundant!
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
		/// <returns>A <see cref="CreatedResult"/> of 201.</returns>
		protected virtual CreatedResult Created(object content) => Created("", content);

		/// <summary>
		/// Creates a 404 NotFound <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 404.</returns>
		protected virtual ContentResult NotFound(string reason) => CreateStringContentResult(404, reason);

		/// <summary>
		/// Creates a 409 Conflict <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 409.</returns>
		protected virtual ContentResult Conflict(string reason) => CreateStringContentResult(409, reason);

		/// <summary>
		/// Creates a 500 InternalServerError <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 500.</returns>
		protected virtual ContentResult InternalServerError(string reason) => CreateStringContentResult(500, reason);

		/// <summary>
		/// Creates a 401 Unauthorized <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 501.</returns>
		protected virtual ContentResult Unauthorized(string reason) => CreateStringContentResult(401, reason);

		/// <summary>
		/// Creates a 403 Forbidden <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 403.</returns>
		protected virtual ContentResult Forbidden(string reason) => CreateStringContentResult(403, reason);

		/// <summary>
		/// Creates a 405 MethodNotAllowed <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 405.</returns>
		protected virtual ContentResult MethodNotAllowed(string reason) => CreateStringContentResult(405, reason);

		/// <summary>
		/// Creates a 429 TooManyRequests <see cref="ContentResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ContentResult"/> of 429.</returns>
		protected virtual ContentResult TooManyRequests(string reason) => CreateStringContentResult(429, reason);

		/// <summary>
		/// Creates a string <see cref="ContentResult"/> with the specified reason and status code.
		/// </summary>
		/// <param name="statusCode">The status code.</param>
		/// <param name="reason">The reason.</param>
		/// <returns>The <see cref="ContentResult"/>.</returns>
		protected virtual ContentResult CreateStringContentResult(int statusCode, string reason) => new ContentResult
		{
			Content = reason,
			ContentType = "text/plain",
			StatusCode = statusCode
		};
		#endregion
	}
}