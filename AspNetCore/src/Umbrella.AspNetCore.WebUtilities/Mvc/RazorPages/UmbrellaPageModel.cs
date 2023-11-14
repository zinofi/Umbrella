using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.RazorPages;

/// <summary>
/// Serves as the base class for all Razor Page models.
/// </summary>
/// <seealso cref="PageModel" />
public abstract class UmbrellaPageModel : PageModel
{
	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaPageModel"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	protected UmbrellaPageModel(
		ILogger logger)
	{
		Logger = logger;
	}
}