using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Umbrella.WebUtilities.Versioning.Abstractions;
using Umbrella.WebUtilities.Versioning.Models;

namespace Umbrella.AspNetCore.WebUtilities.Components;

/// <summary>
/// A component that displays the system version information.
/// </summary>
public abstract class SystemVersionBase : ComponentBase
{
	[Inject]
	private ISystemVersionService SystemVersionService { get; set; } = null!;

	[CascadingParameter]
	private HttpContext HttpContext { get; set; } = null!;

	/// <summary>
	/// Gets the model containing the system version information.
	/// </summary>
	protected SystemVersionModel? Model { get; set; }

	/// <inheritdoc />
	protected override async Task OnInitializedAsync()
	{
		Model = await SystemVersionService.GetAsync(HttpContext.RequestAborted);
	}
}