using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Numerics;
using Umbrella.Utilities.Http.Constants;

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
		protected ILogger Logger { get; }

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
			Logger = logger;
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
		/// <returns>A <see cref="CreatedResult"/> of 201.</returns>
		protected virtual CreatedResult Created(object content) => Created("", content);

		/// <summary>
		/// Creates a 400 BadRequest <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 400.</returns>
		protected virtual ObjectResult BadRequest(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 400, title: "BadRequest", code: code);

		/// <summary>
		/// Creates a 401 Unauthorized <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 401.</returns>
		protected virtual ObjectResult Unauthorized(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 401, title: "Unauthorized", code: code);

		/// <summary>
		/// Creates a 403 Forbidden <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 403.</returns>
		protected virtual ObjectResult Forbidden(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 403, title: "Forbidden", code: code);

		/// <summary>
		/// Creates a 404 NotFound <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 404.</returns>
		protected virtual ObjectResult NotFound(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 404, title: "Not Found", code: code);

		/// <summary>
		/// Creates a 405 MethodNotAllowed <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 405.</returns>
		protected virtual ObjectResult MethodNotAllowed(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 405, title: "Method Not Allowed", code: code);

		/// <summary>
		/// Creates a 409 Conflict <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 409.</returns>
		protected virtual ObjectResult Conflict(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 409, title: "Conflict", code: code);

		/// <summary>
		/// Creates a 409 Conflict <see cref="ObjectResult"/> with the specified reason with the code set to <see cref="HttpProblemCodes.ConcurrencyStampMismatch"/>.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>A <see cref="ObjectResult"/> of 409.</returns>
		protected virtual ObjectResult ConcurrencyConflict(string reason) => UmbrellaProblem(reason, statusCode: 409, title: "Concurrency Conflict", code: HttpProblemCodes.ConcurrencyStampMismatch);

		/// <summary>
		/// Creates a 429 TooManyRequests <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 429.</returns>
		protected virtual ObjectResult TooManyRequests(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 429, title: "Too Many Requests", code: code);

		/// <summary>
		/// Creates a 500 InternalServerError <see cref="ObjectResult"/> with the specified reason.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <param name="code">The error code.</param>
		/// <returns>A <see cref="ObjectResult"/> of 500.</returns>
		protected virtual ObjectResult InternalServerError(string reason, string? code = null) => UmbrellaProblem(reason, statusCode: 500, title: "Error", code: code);

		/// <summary>
		/// Creates an <see cref="ObjectResult"/> containing an <see cref="UmbrellaProblemDetails"/> instance.
		/// </summary>
		/// <param name="detail">The detail.</param>
		/// <param name="instance">The instance.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="title">The title.</param>
		/// <param name="type">The type.</param>
		/// <param name="code">The error code.</param>
		/// <returns>The <see cref="ObjectResult"/> containing an instance of <see cref="UmbrellaProblemDetails"/>.</returns>
		protected virtual ObjectResult UmbrellaProblem(string? detail = null, string? instance = null, int? statusCode = null, string? title = null, string? type = null, string? code = null)
		{
			var problemDetails = new UmbrellaProblemDetails
			{
				Code = code,
				Detail = detail,
				Instance = instance,
				Status = statusCode,
				Title = title,
				Type = type,
				CorrelationId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			};

			return new ObjectResult(problemDetails)
			{
				ContentTypes = { "application/problem+json" },
				StatusCode = statusCode
			};
		}
		#endregion
	}
}