using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Versioning.Abstractions;
using Umbrella.WebUtilities.Versioning.Models;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ViewComponents;

/// <summary>
/// A View Component used to display version information about the system wapped inside HTML comments.
/// </summary>
/// <seealso cref="UmbrellaViewComponent" />
public class SystemVersion : UmbrellaViewComponent
{
	private readonly ISystemVersionService _systemVersionService;

	/// <summary>
	/// Initializes a new instance of the <see cref="SystemVersion"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="systemVersionService">The system version service.</param>
	public SystemVersion(
		ILogger<SystemVersion> logger,
		ISystemVersionService systemVersionService)
		: base(logger)
	{
		_systemVersionService = systemVersionService;
	}

	/// <summary>
	/// Invokes the component. This method is called by the MVC infrastructure at runtime.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The <see cref="IViewComponentResult"/>.</returns>
	/// <exception cref="UmbrellaWebException">There was a problem getting version information.</exception>
	public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			SystemVersionModel model = await _systemVersionService.GetAsync(cancellationToken);

			return View(model);
		}
		catch (Exception exc) when (Logger.WriteError(exc))
		{
			throw new UmbrellaWebException("There was a problem getting version information.", exc);
		}
	}
}